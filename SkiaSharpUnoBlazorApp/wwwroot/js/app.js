window.skiasharpFunctions = {
    redraw: (text, color, size) => {
        var iframe = document.getElementById('skiasharp-iframe');

        var src = iframe.src || "";
        var idx = src.indexOf('#');
        if (idx >= 0) {
            src = src.substr(0, idx);
        }

        iframe.src = `${src}#text=${encodeURIComponent(text)}&color=${encodeURIComponent(color)}&size=${encodeURIComponent(size)}`;
    }
};
