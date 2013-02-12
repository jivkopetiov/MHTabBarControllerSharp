using MonoTouch.UIKit;
using System;

namespace MHTabBarControllerSharp
{
	public class ListViewController : UITableViewController
	{
		public override void ViewDidLoad ()
		{
			TableView.Source = new TableSource(this);
		}

		private class TableSource : UITableViewSource {

			private UIViewController _root;

			public TableSource (UIViewController root)
			{
				_root = root;
			}

			public override int RowsInSection (UITableView tableview, int section)
			{
				return 50;
			}

			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell ("cell");
				if (cell == null)
					cell = new UITableViewCell(UITableViewCellStyle.Default, "cell");
				
				cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
				cell.TextLabel.Text = "Row - " + indexPath.Row;
				return cell;
			}

			public override void RowSelected (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				var listViewController1 = new ListViewController();
				var listViewController2 = new ListViewController();
				
				listViewController1.Title = @"Another Tab 1";
				listViewController2.Title = @"Another Tab 2";
				
				var tabBarController = new MHTabBarController(0);
				tabBarController.SetViewControllers(new[] { listViewController1, listViewController2 });
				tabBarController.Title = @"Modal Screen";
				tabBarController.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
					_root.DismissViewController(true, null);
				});
				
				UINavigationController navController = new UINavigationController(tabBarController);
				navController.NavigationBar.TintColor = UIColor.Black;
				_root.PresentViewController(navController, true, null);
			}
		}
	}
}