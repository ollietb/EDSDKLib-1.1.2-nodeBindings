const path = require('path');
const fs = require('fs');
const edge = require('edge');

const assemblyFile = path.join(__dirname, '../NodeBindings/bin/Debug/NodeBindings.dll');
const className = 'NodeBindings.Program';

const bindMethodSignature = function(methodName) {
    return edge.func({
        assemblyFile: assemblyFile,
        typeName: className,
        methodName: methodName // This must be Func<object,Task<object>>
    });
};

/**
 * @param {string} methodName
 * @return {function(*=): Promise}
 */
const bindPromisify = function(methodName) {
    const func = bindMethodSignature(methodName);

    return (opts = {}) => {
        return new Promise((resolve, reject) => {
            func(opts, (error, result) => {
                if (error) {
                    console.log(`ERROR: ${error.message}`, error);
                    reject(error);
                    return;
                }

                if (result && result.success) {
                    console.log(`Callback on success result: ${result.message}`, result);
                } else {
                    console.log(`Callback on failure result: ${result.message}`, result);
                }
                resolve(result);
            });
        });
    };
};

const setOutputPath = bindPromisify('SetOutputPath');
const takePhoto = bindPromisify('TakePhoto');
const beginSession = bindPromisify('BeginSession');
const endSession = bindPromisify('EndSession');
const startLiveView = bindPromisify('StartLiveView');
const stopLiveView = bindPromisify('StopLiveView');
const startVideo = bindPromisify('StartVideo');
const stopVideo = bindPromisify('StopVideo');
const getLastDownloadedImageFilename = bindPromisify('GetLastDownloadedImageFilename');
const getPreviewImage = bindPromisify('GetPreviewImage');

const benchmark = (message, promiseOrFunction) => {
    const ss = Date.now();

    return Promise.resolve()
        .then(() => typeof promiseOrFunction === 'function' ? promiseOrFunction() : promiseOrFunction)
        .then(result => {
            console.log(`${message}: ${Date.now() - ss}ms`);
            return result;
        });
};
const delay = (time) => new Promise(r => setTimeout(r, time));

const outputPath = 'C:\\pictures';

// Promise.resolve()
//     .then(() => benchmark('beginSession', () => beginSession()))
//     .then(() => benchmark('setOutputPath', () => setOutputPath({ outputPath })))
//     // .then(() => benchmark('takePhoto', () => takePhoto()))
//     .then(() => benchmark('startLiveView', () => startLiveView()))
//     // .then(() => benchmark('getPreviewImage', getPreviewImage()))
//     .then(() => benchmark('getPreviewImage', getPreviewImage()))
//     .then(result => fs.writeFile(
//         path.join(outputPath, `live-preview-${Date.now()}.jpg`),
//         result.bitmap, (err) => {}
//     ))
//     .then(() => benchmark('startVideo', () => startVideo()))
//     .then(() => benchmark('delay', () => delay(2 * 1000)))
//     .then(() => benchmark('stopVideo', () => stopVideo()))
//     .then(() => benchmark('stopLiveView', () => stopLiveView()))
//     .then(() => benchmark('endSession', () => endSession()))
// ;

Promise.resolve()
    .then(() => benchmark('beginSession', () => beginSession()))
    .then(() => benchmark('setOutputPath', () => setOutputPath({ outputPath })))
    .then(() => benchmark('takePhoto', () => takePhoto()))
    .then(() => benchmark('endSession', () => endSession()))
;
