using MonoTouch.UIKit;
using System.Linq;
using System;
using System.Drawing;

namespace MHTabBarControllerSharp 
{

	public class MHTabBarController : UIViewController
	{
		private const int TagOffset = 1000;
		private const float tabBarHeight = 44.0f;

		private UIView tabButtonsContainerView;
		private UIView contentContainerView;
		private UIImageView indicatorImageView;
		private UIViewController[] viewControllers;
		private int selectedIndex;
		private int initialSelectedIndex = -1;

		public MHTabBarController (int initialSelectedIndex)
		{
			this.initialSelectedIndex = initialSelectedIndex;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

			var rect = new RectangleF(0.0f, 0.0f, View.Bounds.Width, tabBarHeight);
			tabButtonsContainerView = new UIView(rect);
			tabButtonsContainerView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			Add(tabButtonsContainerView);

			rect.Y = tabBarHeight;
			rect.Height = View.Bounds.Height - tabBarHeight;
			contentContainerView = new UIView(rect);
			contentContainerView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			Add(contentContainerView);

			indicatorImageView = new UIImageView(UIImage.FromFile("images/MHTabBarIndicator.png"));
			Add(indicatorImageView);

			ReloadTabButtons();

			SetSelectedIndex (initialSelectedIndex, false);

			var swipeLeft = new UISwipeGestureRecognizer((rec) => { 
				if (selectedIndex > 0)
					SetSelectedIndex (selectedIndex - 1, true);
			});
			swipeLeft.Direction = UISwipeGestureRecognizerDirection.Right;
			View.AddGestureRecognizer (swipeLeft);

			var swipeRight = new UISwipeGestureRecognizer((rec) => { 
				if (selectedIndex < viewControllers.Length - 1)
					SetSelectedIndex (selectedIndex + 1, true);
			});
			swipeRight.Direction = UISwipeGestureRecognizerDirection.Left;
			View.AddGestureRecognizer (swipeRight);
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();
			LayoutTabButtons();
		}

		public override bool ShouldAutorotate ()
		{
			foreach(var viewController in viewControllers)
			{
				if (!viewController.ShouldAutorotate())
					return false;
			}
			return true;
		}

		private void ReloadTabButtons()
		{
			RemoveTabButtons();
			AddTabButtons();

			// Force redraw of the previously active tab.
			int lastIndex = selectedIndex;
			selectedIndex = -1;
			selectedIndex = lastIndex;
		}

		private void AddTabButtons()
		{
			int index = 0;
			foreach (var viewController in viewControllers)
			{
				UIButton button = UIButton.FromType(UIButtonType.Custom);
				button.Tag = TagOffset + index;
				button.TitleLabel.Font = UIFont.BoldSystemFontOfSize(18);
				button.TitleLabel.ShadowOffset = new SizeF(0.0f, 1.0f);

				UIOffset offset = viewController.TabBarItem.TitlePositionAdjustment;
				button.TitleEdgeInsets = new UIEdgeInsets(offset.Vertical, offset.Horizontal, 0.0f, 0.0f);
				button.ImageEdgeInsets = viewController.TabBarItem.ImageInsets;
				button.SetTitle(viewController.TabBarItem.Title, UIControlState.Normal);
				button.SetImage(viewController.TabBarItem.Image, UIControlState.Normal);

				button.TouchDown += delegate {
					SetSelectedIndex(button.Tag - TagOffset, true);
				};

				deselectTabButton(button);
				tabButtonsContainerView.AddSubview(button);

				++index;
			}
		}

		private void RemoveTabButtons()
		{
			while (tabButtonsContainerView.Subviews.Length > 0)
			{
				tabButtonsContainerView.Subviews[tabButtonsContainerView.Subviews.Length - 1].RemoveFromSuperview();
			}
		}

		private void LayoutTabButtons()
		{
			int index = 0;
			int count = viewControllers.Length;

			var rect = new RectangleF(0.0f, 0.0f, (float)Math.Floor(View.Bounds.Width / count), tabBarHeight);

			indicatorImageView.Hidden = true;

			var buttons = tabButtonsContainerView.Subviews;
			foreach (UIButton button in buttons)
			{
				if (index == count - 1)
					rect.Width = View.Bounds.Width - rect.X;

				button.Frame = rect;
				rect.X += rect.Width;

				if (index == selectedIndex)
					CenterIndicatorOnButton(button);

				++index;
			}
		}

		private void CenterIndicatorOnButton (UIButton button)
		{
			var rect = indicatorImageView.Frame;
			rect.X = button.Center.X - (float)Math.Floor(indicatorImageView.Frame.Width / 2.0f);
			rect.Y = tabBarHeight - indicatorImageView.Frame.Height;
			indicatorImageView.Frame = rect;
			indicatorImageView.Hidden = false;
		}

		public void SetViewControllers (UIViewController[] newViewControllers)
		{
			if (newViewControllers.Length < 2)
				throw new Exception("MHTabBarController requires at least two view controllers");

			UIViewController oldSelectedViewController = selectedViewController;

			// Remove the old child view controllers.
			if (viewControllers != null) {
				foreach (var viewController in viewControllers)
				{
					viewController.WillMoveToParentViewController(null);
					viewController.RemoveFromParentViewController();
				}
			}

			viewControllers = newViewControllers.ToArray();

			// This follows the same rules as UITabBarController for trying to
			// re-select the previously selected view controller.
			int newIndex = Array.IndexOf (viewControllers, oldSelectedViewController);
			if (newIndex != -1)
				selectedIndex = newIndex;
			else if (newIndex < viewControllers.Length)
				selectedIndex = newIndex;
			else
				selectedIndex = 0;

			// Add the new child view controllers.
			foreach (var viewController in viewControllers)
			{
				AddChildViewController(viewController);
				viewController.DidMoveToParentViewController(this);
			}

			if (IsViewLoaded)
				ReloadTabButtons();
		}

		public void SetSelectedIndex(int newSelectedIndex, bool animated)
		{
			if (newSelectedIndex >= viewControllers.Length)
				throw new Exception("View controller index out of bounds");

			if (!IsViewLoaded)
			{
				selectedIndex = newSelectedIndex;
			}
			else if (selectedIndex != newSelectedIndex)
			{
				UIViewController fromViewController = null;
				UIViewController toViewController = null;

				if (selectedIndex != -1)
				{
					UIButton fromButton = (UIButton)tabButtonsContainerView.ViewWithTag(TagOffset + selectedIndex);
					deselectTabButton(fromButton);
					fromViewController = selectedViewController;
				}

				int oldSelectedIndex = selectedIndex;
				selectedIndex = newSelectedIndex;

				UIButton toButton = null;
				if (selectedIndex != -1)
				{
					toButton = (UIButton)tabButtonsContainerView.ViewWithTag(TagOffset + selectedIndex);
					selectTabButton(toButton);
					toViewController = selectedViewController;
				}

				if (toViewController == null)  // don't animate
				{
					fromViewController.View.RemoveFromSuperview();
				}
				else if (fromViewController == null)  // don't animate
				{
					toViewController.View.Frame = contentContainerView.Bounds;
					contentContainerView.AddSubview(toViewController.View);
					CenterIndicatorOnButton(toButton);
				}
				else if (animated)
				{
					var rect = contentContainerView.Bounds;
					if (oldSelectedIndex < newSelectedIndex)
						rect.X = rect.Width;
					else
						rect.X = -rect.Width;

					toViewController.View.Frame = rect;
					tabButtonsContainerView.UserInteractionEnabled = false;

					this.Transition (
						fromViewController, toViewController, 0.3f, 
						UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.CurveEaseOut,
						delegate
						{
							var newRect = fromViewController.View.Frame;
							if (oldSelectedIndex < newSelectedIndex)
								newRect.X = -newRect.Width;
							else
								newRect.X = newRect.Width;

							fromViewController.View.Frame = newRect;
							toViewController.View.Frame = contentContainerView.Bounds;
							CenterIndicatorOnButton(toButton);
						},
						delegate
						{
							tabButtonsContainerView.UserInteractionEnabled = true;
						});
				}
				else  // not animated
				{
					fromViewController.View.RemoveFromSuperview();

					toViewController.View.Frame = contentContainerView.Bounds;
					contentContainerView.AddSubview(toViewController.View);
					CenterIndicatorOnButton(toButton);
				}
			}
		}

		private UIViewController selectedViewController
		{
			get {
				if (selectedIndex != -1 && viewControllers != null)
					return viewControllers[selectedIndex];
				else
					return null;
			}
		}

		private void selectTabButton (UIButton button)
		{
			button.SetTitleColor(UIColor.Red, UIControlState.Normal);

			UIImage image = UIImage.FromFile("images/MHTabBarActiveTab.png").StretchableImage(0, 0);
			button.SetBackgroundImage(image, UIControlState.Normal);
			button.SetBackgroundImage(image, UIControlState.Highlighted);
			
			button.SetTitleColor(UIColor.White, UIControlState.Normal);
			button.SetTitleShadowColor(UIColor.FromWhiteAlpha(0.0f, 0.5f), UIControlState.Normal);
		}

		private void deselectTabButton (UIButton button)
		{
			button.SetTitleColor(UIColor.Black, UIControlState.Normal);

			UIImage image = UIImage.FromFile("images/MHTabBarInactiveTab.png").StretchableImage(1, 0);
			button.SetBackgroundImage(image, UIControlState.Normal);
			button.SetBackgroundImage(image, UIControlState.Highlighted);

			button.SetTitleColor(UIColor.FromRGBA(175/255.0f, 85/255.0f, 58/255.0f, 1.0f), UIControlState.Normal);
			button.SetTitleShadowColor(UIColor.White, UIControlState.Normal);
		}
	}						
}