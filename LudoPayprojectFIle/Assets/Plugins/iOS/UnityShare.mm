
//  Created by Draft Sama on 9/2/2559 BE.
//  Copyright © 2559 Lighting Bomb. All rights reserved.
//

#import "UnityShare.h"

@implementation UnityShare



-(BOOL)stringIsNilOrEmpty:(NSString*)aString {
    return !(aString && aString.length);
}




-(void)Share:(char *)filesPathJson subject:(char *)subject message:(char *)message{
    
    [NSObject cancelPreviousPerformRequestsWithTarget:self];
    UIViewController *vc = UnityGetGLViewController();

    NSMutableArray *objectsToShare = [[NSMutableArray alloc] init];
    
    
    NSString *filesPathJsonString = [NSString stringWithUTF8String:filesPathJson];
    NSData *filesJsonData = [filesPathJsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSArray *filesList = nil;
    
    filesList = [NSJSONSerialization JSONObjectWithData: filesJsonData options:NSJSONReadingMutableContainers error: nil];
    
    
    
    if(filesList != nil){
        
        for (NSString *filepath in filesList) {
            
           NSURL *url = [NSURL fileURLWithPath:filepath];
            
            
            NSLog(@"FilePath : %@",filepath);

            [objectsToShare addObject:url];
            
         
        }
        
        
    }
    
       
    if(message != nil){
    //Add Message
        NSString* messageObj = [NSString stringWithUTF8String:message];
        
        [objectsToShare addObject:messageObj];
        
    }
    
    
         UIActivityViewController *controller = [[UIActivityViewController alloc] initWithActivityItems:objectsToShare applicationActivities:nil];
    
    //Add Subject
    if(subject != nil){
    
        [controller setValue:[NSString stringWithUTF8String:subject] forKey:@"subject"];
        
    }

  
    
    if(UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
    {
        controller.popoverPresentationController.sourceView = vc.view;
        controller.popoverPresentationController.sourceRect =  CGRectMake(UIScreen.mainScreen.bounds.size.width/2, UIScreen.mainScreen.bounds.size.height,0, 0);
    }
    
    
    
//    NSArray *excludeActivities = @[UIActivityTypeAirDrop,
//                                   UIActivityTypePrint,
//                                   UIActivityTypeAssignToContact,
//                                   UIActivityTypeSaveToCameraRoll,
//                                   UIActivityTypeAddToReadingList,
//                                   UIActivityTypePostToFlickr,
//                                   UIActivityTypePostToVimeo];
//    
//    controller.excludedActivityTypes = excludeActivities;

    [vc presentViewController:controller animated:YES completion:nil];

    
    // check if new API supported
    if ([controller respondsToSelector:@selector(completionWithItemsHandler)]) {
        controller.completionWithItemsHandler = ^(NSString *activityType, BOOL completed, NSArray *returnedItems, NSError *activityError) {
        
            
            if(completed){
                
                //NSLog(@"Send %@ Completed",activityType);
                UnitySendMessage("NativeShareListener", "NativeShare_OnShareCompleted", "1");

            }else{
                
               // NSLog(@"Send %@ Cancel",activityType);
                UnitySendMessage("NativeShareListener", "NativeShare_OnShareCompleted", "2");

            }
            
            
        };
    }
//    else{
//        controller.completionHandler = ^(NSString *activityType, BOOL completed) {
//            // When completed flag is YES, user performed specific activity
//            
//            if(completed){
//                
//                NSLog(@"Send %@ Completed",activityType);
//                
//            }else{
//                
//                NSLog(@"Send %@ Cancel",activityType);
//                
//            }
//        };
//        
//    }
    
    
    

    
}



-(void)SendLine:(char *)filesPathJson message:(char *)message{
    
    NSLog(@"SendLine");

//    <key>LSApplicationQueriesSchemes</key>
//    <array>
//    <string>line</string>
//    </array>
//
    
    NSArray *filesList = [self GetFileListFormJson:filesPathJson];

    
    //ADD file
//    NSString *filesPathJsonString = [NSString stringWithUTF8String:filesPathJson];
//    NSData *filesJsonData = [filesPathJsonString dataUsingEncoding:NSUTF8StringEncoding];
//    NSArray *filesList = nil;
//
//    filesList = [NSJSONSerialization JSONObjectWithData: filesJsonData options:NSJSONReadingMutableContainers error: nil];
//
    
  //  NSLog(@"filesList %d",[filesList count]);

    if(filesList != nil){
      
        //NSArray<UIImage*> *imagePaths = [[NSArray<UIImage*> alloc]init];
        
        NSMutableArray * imageViewsArray = [[NSMutableArray alloc] init];

        
          for (NSString *filepath in filesList) {
              
              UIImage *image =[UIImage imageWithContentsOfFile:filepath];
              
            //  NSLog(@"ADD : %@", image);

              [imageViewsArray addObject:image];

              
          }

     
        
        UIPasteboard *pasteboard;
        if ([[UIDevice currentDevice].systemVersion floatValue] >= 7.0f) {
            pasteboard = [UIPasteboard generalPasteboard];
        } else {
            pasteboard = [UIPasteboard pasteboardWithUniqueName];
        }
        
        [pasteboard setImages:imageViewsArray];

        
        
        //single
       // [pasteboard setImages:imagePaths];
        
//        NSString *extension = [[@"." stringByAppendingString:[imagePath pathExtension]] lowercaseString];
//        if([extension rangeOfString:@".png"].location != NSNotFound) {
//            //[pasteboard setData:UIImagePNGRepresentation([UIImage imageWithContentsOfFile:imagePath]) forPasteboardType:@"public.png"];
//        } else {
//            //[pasteboard setData:UIImageJPEGRepresentation([UIImage imageWithContentsOfFile:imagePath], 1.0f) forPasteboardType:@"public.jpeg"];
//        }
        
          NSString *url = [NSString stringWithFormat:@"line://msg/image/%@", pasteboard.name];
        

        NSURL *urlData = [NSURL URLWithString:url];
        if ([[UIApplication sharedApplication] canOpenURL:urlData]) {
            [[UIApplication sharedApplication] openURL:urlData];
            UnitySendMessage("NativeShareListener", "NativeShare_OnShareCompleted", "1");
        } else {
            UnitySendMessage("NativeShareListener", "NativeShare_OnShareCompleted", "2");
        }
        
    }
    
    
    
    
    
}



-(NSArray*)GetFileListFormJson:(char*)filesPathJson{
    
    NSString *filesPathJsonString = [NSString stringWithUTF8String:filesPathJson];
    NSData *filesJsonData = [filesPathJsonString dataUsingEncoding:NSUTF8StringEncoding];
    
 return [NSJSONSerialization JSONObjectWithData: filesJsonData options:NSJSONReadingMutableContainers error: nil];
    
}

-(void)SocialShare:(char *)filesPathJson subject:(char*)subject  message:(char *)message socialApp:(char *)socialApp{
 
    
    
    
// Share Image only
    NSString* social = [NSString stringWithUTF8String:socialApp];
    //Share Line
    if([social isEqualToString:@"jp.naver.line.Share"]){
        [self SendLine:filesPathJson message:message];
        return;
    }


    
    if(social != nil){
        
        if ([SLComposeViewController isAvailableForServiceType:social]) {
            
            SLComposeViewController *mySLComposerSheet = [SLComposeViewController composeViewControllerForServiceType:social];
            
            
            
            if([mySLComposerSheet serviceType] == NULL){
                
                NSLog(@"[Native Share] %@  not install",social);
                
                UnitySendMessage("NativeShareListener", "NativeShare_OnShareCompleted", "3");
                
                
                [self Share:filesPathJson subject:subject message:message];
                
                return;
            }else{
                
                NSLog(@"%@",[mySLComposerSheet serviceType]);
                
            }
            
            
            
            if(message != nil){
                
                NSString *messageFinal = [NSString stringWithUTF8String:message];
                [mySLComposerSheet setInitialText:messageFinal];
            }
            
            
            
            
            //ADD file
            NSString *filesPathJsonString = [NSString stringWithUTF8String:filesPathJson];
            NSData *filesJsonData = [filesPathJsonString dataUsingEncoding:NSUTF8StringEncoding];
            NSArray *filesList = nil;
            
            filesList = [NSJSONSerialization JSONObjectWithData: filesJsonData options:NSJSONReadingMutableContainers error: nil];
            
            
            
            if(filesList != nil){
                
                for (NSString *filepath in filesList) {
                    
                    
                    
                    // NSLog(@"============= %@ ================",[filepath pathExtension]);
                    
                    if([[filepath pathExtension] isEqualToString:@"png"] || [[filepath pathExtension] isEqualToString:@"jpg"]|| [[filepath pathExtension] isEqualToString:@"gif"]){
                        
                        NSLog(@"Path: %@",filepath);
                        
                        
                        [mySLComposerSheet addImage:[UIImage imageWithContentsOfFile:filepath]];
                    }else{
                        
                        [mySLComposerSheet addURL:[NSURL fileURLWithPath:filepath]];
                    }
                    
                    
                    
                }
                
                
            }
            
            
            
            //               NSString *urlFinal = [NSString stringWithUTF8String:url];
            //
            //             [mySLComposerSheet addURL:[NSURL URLWithString:urlFinal]];
            
            UIViewController *vc = UnityGetGLViewController();
            [vc presentViewController:mySLComposerSheet animated:YES completion:nil];
            
            [mySLComposerSheet setCompletionHandler:^(SLComposeViewControllerResult result) {
                
                switch (result) {
                    case SLComposeViewControllerResultCancelled:
                        //NSLog(@"Post Canceled");
                        UnitySendMessage("NativeShareListener", "NativeShare_OnShareCompleted", "2");
                        
                        break;
                    case SLComposeViewControllerResultDone:
                        // NSLog(@"Post Sucessful");
                        UnitySendMessage("NativeShareListener", "NativeShare_OnShareCompleted", "1");
                        
                        break;
                        
                    default:
                        break;
                }
            }];
            
            
            
        }
        
        
        
    }
    
    
    
    
    
    
}



-(void)SendEmail:(char*)subject message:(char*)message attachFilesJson:(char*)attachFilesJson emailsJson:(char*)emailsJson{
    
    
    NSError *error = nil;
    NSString *emailsJsonString = [NSString stringWithUTF8String:emailsJson];
    NSString *attachFilesJsonString = [NSString stringWithUTF8String:attachFilesJson];
    NSString *emailSubject =[NSString stringWithUTF8String:subject];
    NSString *emailMessage =[NSString stringWithUTF8String:message];
    NSArray *toRecipents = nil;
    
    //Json Encode
   /*
    NSArray *arrayData = @[@"cinowacs@gamil.com",@"test@test.com"];
    NSData *jsonDataEncode = [NSJSONSerialization dataWithJSONObject:arrayData options:NSJSONWritingPrettyPrinted error:nil];
    jsonString = [[NSString alloc] initWithData:jsonDataEncode encoding:NSUTF8StringEncoding];
    
    
 
    NSLog(@"Json Encode ===== %@ =========",jsonString);
   
    */

    
    //Json Decode
   
    
    
   NSData *emailsJsonData = [emailsJsonString dataUsingEncoding:NSUTF8StringEncoding];
    
    toRecipents = [NSJSONSerialization JSONObjectWithData: emailsJsonData options:NSJSONReadingMutableContainers error: &error];
    
    
    
    
    
    
    MFMailComposeViewController *mc = [[MFMailComposeViewController alloc] init];
    
    
    if(![MFMailComposeViewController canSendMail]){
        
        NSLog(@"[Native Share] Mail not install");

        UnitySendMessage("NativeShareListener", "NativeShare_OnShareCompleted", "3");

        return;
    }
    
  
    if(emailSubject != nil){
        [mc setSubject:emailSubject];
    }
    if(emailMessage != nil){
        [mc setMessageBody:emailMessage isHTML:NO];
    }
    
 
     if(toRecipents != nil){
         [mc setToRecipients:toRecipents];
  
    }

    
    if(![self stringIsNilOrEmpty:attachFilesJsonString]){
    
    
    
    NSData *filesJsonData = [attachFilesJsonString dataUsingEncoding:NSUTF8StringEncoding];
    
   NSArray *filesList = [NSJSONSerialization JSONObjectWithData: filesJsonData options:NSJSONReadingMutableContainers error: &error];

    for (NSString *filepath in filesList) {
      
         NSURL *url = [NSURL fileURLWithPath:filepath];
        NSData *file = [NSData dataWithContentsOfURL:url];

        [mc addAttachmentData:file mimeType:@"*/*" fileName:  [url lastPathComponent]];

        
    }
    
    }
    
    
     // Present mail view controller on screen
    UIViewController *vc = UnityGetGLViewController();

    mc.mailComposeDelegate =self;
    [vc presentViewController:mc animated:YES completion:NULL];
    
}



- (void)mailComposeController:(MFMailComposeViewController*)controller didFinishWithResult:(MFMailComposeResult)result error:(NSError*)error {
    
    
    if(result == MFMailComposeResultSent)
        UnitySendMessage("NativeShareListener", "NativeShare_OnShareCompleted", "1");
    else if(result == MFMailComposeResultCancelled)
        UnitySendMessage("NativeShareListener", "NativeShare_OnShareCompleted", "2");
    else if(result == MFMailComposeResultFailed)
        UnitySendMessage("NativeShareListener", "NativeShare_OnShareCompleted", "3");

    
   
    
      UIViewController *vc = UnityGetGLViewController();
        [vc dismissViewControllerAnimated:YES completion:NULL];
}
@end


static UnityShare* native = nil;
extern "C"
{
    
    
    
    void Unity_Share(char* filesPathJson,char* subject ,char* message ,char* emailsJson ,char* packageName){
        
        
        if(native == nil)
            native = [[UnityShare alloc] init];
        
        
        NSString *app = [NSString stringWithUTF8String:packageName];
        if([native stringIsNilOrEmpty:app]){

            [native Share:filesPathJson subject:subject message:message];
        }else if([app  isEqual: @"com.apple.mobilemail"]){
            [native SendEmail:subject message:message attachFilesJson:filesPathJson emailsJson:emailsJson];
        }else{
            
            [native SocialShare:filesPathJson subject:subject message:message socialApp:packageName];
        }
        
        
    }
    
    
    
   
    
   
    
}
