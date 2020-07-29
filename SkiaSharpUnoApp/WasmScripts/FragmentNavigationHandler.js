var SkiaSharpUnoApp;
(function (SkiaSharpUnoApp) {
    class FragmentNavigationHandler {
        static activeInstances = {};

        constructor(managedHandle) {
            this.managedHandle = managedHandle;

            this.currentFragment = undefined;
            this.subscribed = false;
        }

        // JSObject
        static createInstance(managedHandle, jsHandle) {
            FragmentNavigationHandler.activeInstances[jsHandle] = new FragmentNavigationHandler(managedHandle);
        }
        static getInstance(jsHandle) {
            return FragmentNavigationHandler.activeInstances[jsHandle];
        }
        static destroyInstance(jsHandle) {
            delete FragmentNavigationHandler.activeInstances[jsHandle];
        }

        //

        getCurrentFragment() {
            return window.location.hash;
        }

        setCurrentFragment(fragment) {
            window.location.hash = fragment;
            this.currentFragment = window.location.hash;
        }

        subscribeToFragmentChanged() {
            if (this.subscribed)
                return;

            this.subscribed = true;
            this.currentFragment = this.getCurrentFragment();
            window.addEventListener("hashchange", _ => this.notifyFragmentChanged(), false);
        }

        notifyFragmentChanged() {
            const newFragment = this.getCurrentFragment();
            if (newFragment === this.currentFragment)
                return;

            this.currentFragment = newFragment;

            Uno.Foundation.Interop.ManagedObject.dispatch(this.managedHandle, 'OnFragmentChanged', newFragment);
        }
    }

    SkiaSharpUnoApp.FragmentNavigationHandler = FragmentNavigationHandler;

})(SkiaSharpUnoApp || (SkiaSharpUnoApp = {}));
