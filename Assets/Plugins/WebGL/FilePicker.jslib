mergeInto(LibraryManager.library, {
    DownloadFile: function(fileName, base64Content, mimeType) {
        try {
            var fileNameStr = UTF8ToString(fileName);
            var base64Str = UTF8ToString(base64Content);
            var mimeTypeStr = UTF8ToString(mimeType);

            var blob = new Blob([Uint8Array.from(atob(base64Str), c => c.charCodeAt(0))], { type: mimeTypeStr });
            var url = URL.createObjectURL(blob);
            var link = document.createElement('a');
            link.href = url;
            link.download = fileNameStr;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            URL.revokeObjectURL(url);
        } catch (e) {
            console.error("DownloadFile error: " + e.message);
        }
    },

    RequestFile: function(callbackObjectName, callbackMethodName, extensions) {
        var objectName = UTF8ToString(callbackObjectName);
        var methodName = UTF8ToString(callbackMethodName);
        var ext = UTF8ToString(extensions);

        var fileuploader = document.getElementById('fileuploader');
        if (fileuploader) {
            fileuploader.remove();
        }

        fileuploader = document.createElement('input');
        fileuploader.setAttribute('style', 'display:none;');
        fileuploader.setAttribute('type', 'file');
        fileuploader.setAttribute('id', 'fileuploader');
        fileuploader.setAttribute('class', '');
        if (ext && ext.trim() !== '') {
            fileuploader.setAttribute('accept', ext);
        }
        document.body.appendChild(fileuploader);

        fileuploader.onchange = function(e) {
            var files = e.target.files;
            if (files.length === 0) {
                console.log('No file selected');
                SendMessage(objectName, methodName, "");
                fileuploader.remove();
                return;
            }
            var file = files[0];
            var reader = new FileReader();
            reader.onload = function(e) {
                var base64 = e.target.result.split(',')[1]; // Удаляем префикс data:image/*;base64,
                SendMessage(objectName, methodName, base64);
                fileuploader.remove();
            };
            reader.onerror = function(e) {
                console.error("FileReader error: " + e.target.error);
                SendMessage(objectName, methodName, "");
                fileuploader.remove();
            };
            reader.readAsDataURL(file);
        };

        fileuploader.click();
    },

    SaveImageToIndexedDB: function(fileName, base64Data, dataLength) {
        var dbName = "UnityImageDB";
        var storeName = "Images";
        var fileNameStr = UTF8ToString(fileName);
        var base64Str = UTF8ToString(base64Data);

        var request = indexedDB.open(dbName, 1);

        request.onupgradeneeded = function(event) {
            var db = event.target.result;
            if (!db.objectStoreNames.contains(storeName)) {
                db.createObjectStore(storeName);
            }
        };

        request.onsuccess = function(event) {
            var db = event.target.result;
            var transaction = db.transaction([storeName], "readwrite");
            var store = transaction.objectStore(storeName);
            var putRequest = store.put(base64Str, fileNameStr);

            putRequest.onsuccess = function() {
                console.log("Image saved to IndexedDB: " + fileNameStr);
            };
            putRequest.onerror = function(event) {
                console.error("IndexedDB put error: " + event.target.error);
            };
        };

        request.onerror = function(event) {
            console.error("IndexedDB open error: " + event.target.error);
        };
    },

    LoadImageFromIndexedDB: function(fileName, callback) {
        var dbName = "UnityImageDB";
        var storeName = "Images";
        var fileNameStr = UTF8ToString(fileName);

        var request = indexedDB.open(dbName, 1);

        request.onupgradeneeded = function(event) {
            var db = event.target.result;
            if (!db.objectStoreNames.contains(storeName)) {
                db.createObjectStore(storeName);
            }
        };

        request.onsuccess = function(event) {
            var db = event.target.result;
            var transaction = db.transaction([storeName], "readonly");
            var store = transaction.objectStore(storeName);
            var getRequest = store.get(fileNameStr);

            getRequest.onsuccess = function(event) {
                var data = event.target.result;
                if (data) {
                    var buffer = _malloc(lengthBytesUTF8(data) + 1);
                    stringToUTF8(data, buffer, lengthBytesUTF8(data) + 1);
                    dynCall_vi(callback, buffer);
                    _free(buffer);
                } else {
                    var errorMsg = "Error: File not found in IndexedDB for " + fileNameStr;
                    var buffer = _malloc(lengthBytesUTF8(errorMsg) + 1);
                    stringToUTF8(errorMsg, buffer, lengthBytesUTF8(errorMsg) + 1);
                    dynCall_vi(callback, buffer);
                    _free(buffer);
                }
            };

            getRequest.onerror = function(event) {
                var errorMsg = "IndexedDB get error for " + fileNameStr + ": " + event.target.error;
                var buffer = _malloc(lengthBytesUTF8(errorMsg) + 1);
                stringToUTF8(errorMsg, buffer, lengthBytesUTF8(errorMsg) + 1);
                dynCall_vi(callback, buffer);
                _free(buffer);
            };
        };

        request.onerror = function(event) {
            var errorMsg = "IndexedDB open error for " + fileNameStr + ": " + event.target.error;
            var buffer = _malloc(lengthBytesUTF8(errorMsg) + 1);
            stringToUTF8(errorMsg, buffer, lengthBytesUTF8(errorMsg) + 1);
            dynCall_vi(callback, buffer);
            _free(buffer);
        };
    }
});