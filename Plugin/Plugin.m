#import <AVFoundation/AVFoundation.h>

// Internal objects
static AVAssetWriter* _writer;
static AVAssetWriterInput* _writerInput;
static AVAssetWriterInputPixelBufferAdaptor* _bufferAdaptor;

extern void VideoWriter_Start(const char* filePath, int width, int height)
{
    if (_writer)
    {
        NSLog(@"Recording has already been initiated.");
        return;
    }
    
    // Asset writer setup
    NSURL* filePathURL =
      [NSURL fileURLWithPath:[NSString stringWithUTF8String:filePath]];

    _writer =
      [[AVAssetWriter alloc] initWithURL: filePathURL
                                fileType: AVFileTypeQuickTimeMovie
                                   error: NULL];

    // Asset writer input setup
    NSDictionary* settings =
      @{ AVVideoCodecKey: AVVideoCodecTypeH264,
         AVVideoWidthKey: @(width),
        AVVideoHeightKey: @(height) };

    _writerInput =
      [[AVAssetWriterInput assetWriterInputWithMediaType: AVMediaTypeVideo
                                          outputSettings: settings] retain];
    _writerInput.expectsMediaDataInRealTime = true;

    [_writer addInput:_writerInput];

    // Pixel buffer adaptor setup
    NSDictionary* attribs =
      @{ (NSString*)kCVPixelBufferPixelFormatTypeKey: @(kCVPixelFormatType_32BGRA),
                   (NSString*)kCVPixelBufferWidthKey: @(width),
                  (NSString*)kCVPixelBufferHeightKey: @(height) };

    _bufferAdaptor =
      [[AVAssetWriterInputPixelBufferAdaptor
         assetWriterInputPixelBufferAdaptorWithAssetWriterInput: _writerInput
                                    sourcePixelBufferAttributes: attribs] retain];
    
    // Recording start
    [_writer startWriting];
    [_writer startSessionAtSourceTime:kCMTimeZero];
}

extern void VideoWriter_Update(const void* source, uint32_t size, double time)
{
    if (!_writer)
    {
        NSLog(@"Recording hasn't been initiated.");
        return;
    }

    if (!_writerInput.isReadyForMoreMediaData)
    {
        NSLog(@"Writer is not ready.");
        return;
    }
    
    // Buffer allocation
    CVPixelBufferRef buffer;
    CVReturn ret = CVPixelBufferPoolCreatePixelBuffer
      (NULL, _bufferAdaptor.pixelBufferPool, &buffer);

    if (ret != kCVReturnSuccess)
    {
        NSLog(@"Can't allocate a pixel buffer.");
        return;
    }

    // Buffer update
    CVPixelBufferLockBaseAddress(buffer, 0);

    void* pointer = CVPixelBufferGetBaseAddress(buffer);
    size_t buffer_size = CVPixelBufferGetDataSize(buffer);
    memcpy(pointer, source, MIN(size, buffer_size));
    
    // Buffer submission
    [_bufferAdaptor appendPixelBuffer:buffer
                 withPresentationTime:CMTimeMakeWithSeconds(time, 240)];

    CVPixelBufferUnlockBaseAddress(buffer, 0);
}

extern void VideoWriter_End(void)
{
    if (!_writer)
    {
        NSLog(@"Recording hasn't been initiated.");
        return;
    }

    [_writerInput markAsFinished];
    [_writer finishWritingWithCompletionHandler:^{ NSLog(@"Done"); }];

    [_writer release];
    [_writerInput release];
    [_bufferAdaptor release];
    
    _writer = NULL;
    _writerInput = NULL;
    _bufferAdaptor = NULL;
}
