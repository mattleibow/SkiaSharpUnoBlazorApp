var SkiaSharpUnoApp;
(function (SkiaSharpUnoApp) {
    class SkiaSharpFunctions {
        static activeInstances = {};
        static current = undefined;

        constructor(managedHandle) {
            this.managedHandle = managedHandle;

            this.currentFragment = undefined;
            this.subscribed = false;
        }

        // JSObject
        static createInstance(managedHandle, jsHandle) {
            SkiaSharpFunctions.activeInstances[jsHandle] = new SkiaSharpFunctions(managedHandle);
        }
        static getInstance(jsHandle) {
            return SkiaSharpFunctions.activeInstances[jsHandle];
        }
        static destroyInstance(jsHandle) {
            delete SkiaSharpFunctions.activeInstances[jsHandle];
        }

        //

        setCurrent() {
            SkiaSharpFunctions.current = this;
        }

        sendData(data) {
            data = JSON.stringify(data);
            Uno.Foundation.Interop.ManagedObject.dispatch(this.managedHandle, 'OnDataReceived', data);
        }
    }

    SkiaSharpUnoApp.SkiaSharpFunctions = SkiaSharpFunctions;

})(SkiaSharpUnoApp || (SkiaSharpUnoApp = {}));
