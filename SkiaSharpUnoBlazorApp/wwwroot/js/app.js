window.skiasharpFunctions = {
    sendData: (data) => {
        var iframe = document.getElementById('skiasharp-iframe');

        if (!iframe ||
            !iframe.contentWindow ||
            !iframe.contentWindow.skiasharpFunctions ||
            !iframe.contentWindow.skiasharpFunctions.sendData ||
            typeof (iframe.contentWindow.skiasharpFunctions.sendData) != 'function')
            return;

        iframe.contentWindow.skiasharpFunctions.sendData(data);
    },
};
