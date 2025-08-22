mergeInto(LibraryManager.library, {
    RequestFile: function(callbackObjectName, callbackMethodName, extensions) {
        var objectName = UTF8ToString(callbackObjectName);
        var methodName = UTF8ToString(callbackMethodName);
        var ext = UTF8ToString(extensions);

        var fileuploader = document.getElementById('fileuploader');
        if (!fileuploader) {
            fileuploader = document.createElement('input');
            fileuploader.setAttribute('style', 'display:none;');
            fileuploader.setAttribute('type', 'file');
            fileuploader.setAttribute('id', 'fileuploader');
            fileuploader.setAttribute('class', '');
            document.getElementsByTagName('body')[0].appendChild(fileuploader);

            fileuploader.onchange = function(e) {
                var files = e.target.files;
                if (files.length === 0) {
                    console.log('No file selected');
                    return;
                }
                var file = files[0];
                var url = URL.createObjectURL(file);
                SendMessage(objectName, methodName, url);
            };
        }

        if (ext && ext.trim() !== '') {
            fileuploader.setAttribute('accept', ext);
        }
        fileuploader.click();
    }
});