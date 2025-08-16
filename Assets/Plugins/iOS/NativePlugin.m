#import <StoreKit/StoreKit.h>
#import <UIKit/UIKit.h>
#import <MessageUI/MessageUI.h>

@interface NativePlugin : NSObject <MFMailComposeViewControllerDelegate>
@end

@implementation NativePlugin

+ (NativePlugin *)sharedInstance {
 static NativePlugin *sharedInstance = nil;
 static dispatch_once_t onceToken;
 dispatch_once(&onceToken, ^{
 sharedInstance = [[NativePlugin alloc] init];
 });
 return sharedInstance;
}

- (void)mailComposeController:(MFMailComposeViewController *)controller didFinishWithResult:(MFMailComposeResult)result error:(NSError *)error {
 [controller dismissViewControllerAnimated:YES completion:nil];
}

@end

extern "C" {
 void RateAppiOS() {
 if (@available(iOS 10.3, *)) {
 [SKStoreReviewController requestReview];
 } else {
 NSString *appId = [[NSBundle mainBundle] bundleIdentifier];
 NSString *url = [NSString stringWithFormat:@"itms-apps://itunes.apple.com/app/id%@?action=write-review", appId];
 [[UIApplication sharedApplication] openURL:[NSURL URLWithString:url] options:@{} completionHandler:nil];
 }
 }

 void ShareTextiOS(const char* text, const char* subject) {
 NSString *nsText = text ? [NSString stringWithUTF8String:text] : @"";
 NSString *nsSubject = subject ? [NSString stringWithUTF8String:subject] : @"";
 NSArray *items = @[nsText];
 UIActivityViewController *activityVC = [[UIActivityViewController alloc] initWithActivityItems:items applicationActivities:nil];
 if (nsSubject.length > 0) {
 [activityVC setValue:nsSubject forKey:@"subject"];
 }

 UIViewController *rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
 [rootVC presentViewController:activityVC animated:YES completion:nil];
 }

 void ShowToastiOS(const char* message, float duration) {
 NSString *nsMessage = message ? [NSString stringWithUTF8String:message] : @"";
 
 UIWindow *window = [UIApplication sharedApplication].keyWindow;
 UIView *toastView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, window.frame.size.width - 40, 50)];
 toastView.backgroundColor = [UIColor darkGrayColor];
 toastView.layer.cornerRadius = 10;
 toastView.center = window.center;
 toastView.alpha = 0.0;
 
 UILabel *label = [[UILabel alloc] initWithFrame:toastView.bounds];
 label.text = nsMessage;
 label.textColor = [UIColor whiteColor];
 label.textAlignment = NSTextAlignmentCenter;
 label.numberOfLines = 0;
 [toastView addSubview:label];
 
 [window addSubview:toastView];
 
 [UIView animateWithDuration:0.3 animations:^{
 toastView.alpha = 1.0;
 } completion:^(BOOL finished) {
 [UIView animateWithDuration:0.3
 delay:duration
 options:0
 animations:^{
 toastView.alpha = 0.0;
 } completion:^(BOOL finished) {
 [toastView removeFromSuperview];
 }];
 }];
 }

 void OpenEmailiOS(const char* emailAddress, const char* subject, const char* body) {
 NSString *nsEmailAddress = emailAddress ? [NSString stringWithUTF8String:emailAddress] : @"";
 NSString *nsSubject = subject ? [NSString stringWithUTF8String:subject] : @"";
 NSString *nsBody = body ? [NSString stringWithUTF8String:body] : @"";

 if ([MFMailComposeViewController canSendMail]) {
 MFMailComposeViewController *mailVC = [[MFMailComposeViewController alloc] init];
 mailVC.mailComposeDelegate = [NativePlugin sharedInstance];
 [mailVC setToRecipients:@[nsEmailAddress]];
 if (nsSubject.length > 0) {
 [mailVC setSubject:nsSubject];
 }
 if (nsBody.length > 0) {
 [mailVC setMessageBody:nsBody isHTML:NO];
 }

 UIViewController *rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
 [rootVC presentViewController:mailVC animated:YES completion:nil];
 } else {
 NSString *encodedSubject = [nsSubject stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]];
 NSString *encodedBody = [nsBody stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]];
 NSString *urlString = [NSString stringWithFormat:@"mailto:%@?subject=%@&body=%@", nsEmailAddress, encodedSubject, encodedBody];
 [[UIApplication sharedApplication] openURL:[NSURL URLWithString:urlString] options:@{} completionHandler:nil];
 }
 }

 void ShareImageiOS(const char* imagePath, const char* text, const char* subject) {
 NSString *nsImagePath = imagePath ? [NSString stringWithUTF8String:imagePath] : @"";
 NSString *nsText = text ? [NSString stringWithUTF8String:text] : @"";
 NSString *nsSubject = subject ? [NSString stringWithUTF8String:subject] : @"";
 
 NSMutableArray *items = [NSMutableArray array];
 if (nsText.length > 0) {
 [items addObject:nsText];
 }
 if (nsImagePath.length > 0) {
 UIImage *image = [UIImage imageWithContentsOfFile:nsImagePath];
 if (image) {
 [items addObject:image];
 }
 }
 
 UIActivityViewController *activityVC = [[UIActivityViewController alloc] initWithActivityItems:items applicationActivities:nil];
 if (nsSubject.length > 0) {
 [activityVC setValue:nsSubject forKey:@"subject"];
 }
 
 UIViewController *rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
 [rootVC presentViewController:activityVC animated:YES completion:nil];
 }

 void ShareFileiOS(const char* filePath, const char* text, const char* subject) {
 NSString *nsFilePath = filePath ? [NSString stringWithUTF8String:filePath] : @"";
 NSString *nsText = text ? [NSString stringWithUTF8String:text] : @"";
 NSString *nsSubject = subject ? [NSString stringWithUTF8String:subject] : @"";
 
 NSMutableArray *items = [NSMutableArray array];
 if (nsText.length > 0) {
 [items addObject:nsText];
 }
 if (nsFilePath.length > 0) {
 NSURL *fileURL = [NSURL fileURLWithPath:nsFilePath];
 if (fileURL) {
 [items addObject:fileURL];
 }
 }
 
 UIActivityViewController *activityVC = [[UIActivityViewController alloc] initWithActivityItems:items applicationActivities:nil];
 if (nsSubject.length > 0) {
 [activityVC setValue:nsSubject forKey:@"subject"];
 }
 
 UIViewController *rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
 [rootVC presentViewController:activityVC animated:YES completion:nil];
 }

 void OpenEmailMultipleiOS(const char* emailAddresses, const char* subject, const char* body) {
 NSString *nsEmailAddresses = emailAddresses ? [NSString stringWithUTF8String:emailAddresses] : @"";
 NSString *nsSubject = subject ? [NSString stringWithUTF8String:subject] : @"";
 NSString *nsBody = body ? [NSString stringWithUTF8String:body] : @"";
 
 NSArray *recipients = [nsEmailAddresses componentsSeparatedByString:@","];
 
 if ([MFMailComposeViewController canSendMail]) {
 MFMailComposeViewController *mailVC = [[MFMailComposeViewController alloc] init];
 mailVC.mailComposeDelegate = [NativePlugin sharedInstance];
 [mailVC setToRecipients:recipients];
 if (nsSubject.length > 0) {
 [mailVC setSubject:nsSubject];
 }
 if (nsBody.length > 0) {
 [mailVC setMessageBody:nsBody isHTML:NO];
 }
 
 UIViewController *rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
 [rootVC presentViewController:mailVC animated:YES completion:nil];
 } else {
 NSString *encodedSubject = [nsSubject stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]];
 NSString *encodedBody = [nsBody stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]];
 NSString *emailList = [recipients componentsJoinedByString:@","];
 NSString *urlString = [NSString stringWithFormat:@"mailto:%@?subject=%@&body=%@", emailList, encodedSubject, encodedBody];
 [[UIApplication sharedApplication] openURL:[NSURL URLWithString:urlString] options:@{} completionHandler:nil];
 }
 }
}