###Overview

MHTabBarControllerSharp is a MonoTouch port of MHTabBarController (http://github.com/hollance/mhtabbarcontroller). It was originally ported line by line from Obj-C to MonoTouch, but eventually some changes were made:
- Added Swipe Left and Swipe Right gesture recognizers to navigate between the controllers
- Some modifications and simplifications of which method is public and what private fields are available


###Usage
Create the child controllers and the tabbar controller, the int parameter is the index of the selected child controller:
```
var childControllers = new[] { childController1, childController2, childController3 };
var tabbarController = new MHTabBarController(0);
tabbarController.SetViewControllers(childControllers);
window.RootViewController = tabbarController;
```

See AppDelegate.cs and run the app in MonoDevelop for a working example of how to use the controller.



###Licensing
The source code is licensed under the terms of the MIT license.

The original MHTabBarController source code is copyright 2011-2012 Matthijs Hollemans and is licensed under the terms of the MIT license. Below is a copy of the original code license:

>Copyright (c) 2011-2012 Matthijs Hollemans

>Permission is hereby granted, free of charge, to any person obtaining a copy
>of this software and associated documentation files (the "Software"), to deal
>in the Software without restriction, including without limitation the rights
>to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
>copies of the Software, and to permit persons to whom the Software is
>furnished to do so, subject to the following conditions:

>The above copyright notice and this permission notice shall be included in
>all copies or substantial portions of the Software.
 
>THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
>IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
>FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
>AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
>LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
>OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
>THE SOFTWARE.

</notextile>
