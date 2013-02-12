using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MHTabBarControllerSharp
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}

		UIWindow window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			var listViewController1 = new ListViewController();
			var listViewController2 = new ListViewController();
			var listViewController3 = new ListViewController();
			
			listViewController1.Title = "Tab 1";
			listViewController2.Title = "Tab 2";
			listViewController3.Title = "Tab 3";
			
			listViewController2.TabBarItem.Image = UIImage.FromFile("images/Taijitu.png");
			listViewController2.TabBarItem.ImageInsets = new UIEdgeInsets(0.0f, -4.0f, 0.0f, 0.0f);
			listViewController2.TabBarItem.TitlePositionAdjustment = new UIOffset(4.0f, 0.0f);

			var viewControllers = new[] { listViewController1, listViewController2, listViewController3 };
			MHTabBarController tabBarController = new MHTabBarController(0);
			tabBarController.SetViewControllers(viewControllers);

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.BackgroundColor = UIColor.White;
			window.RootViewController = tabBarController;
			window.MakeKeyAndVisible ();
			return true;
		}
	}
}

