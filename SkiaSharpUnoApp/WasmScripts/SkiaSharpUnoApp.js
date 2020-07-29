window.skiasharpFunctions = {
    sendData: (data) => {
        var functions = SkiaSharpUnoApp.SkiaSharpFunctions.current;
        if (!functions)
            return;

        functions.sendData(data);
    },
};
