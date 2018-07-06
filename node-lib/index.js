const path = require('path');
const edge = require('edge');

const assemblyFile = path.join(__dirname, 'prebuilt/NodeBindings.dll');
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

                if (!result || !result.success) {
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
const callCameraMethod = bindPromisify('CallCameraMethod');

module.exports = {
    setOutputPath,
    takePhoto,
    beginSession,
    endSession,
    startLiveView,
    stopLiveView,
    startVideo,
    stopVideo,
    getLastDownloadedImageFilename,
    getPreviewImage,
    callCameraMethod,
};