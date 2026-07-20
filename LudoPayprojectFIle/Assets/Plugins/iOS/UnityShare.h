//
//  UnityBrigde.hpp
//  UnitySharePlugin
//
//  Created by Draft Sama on 9/2/2559 BE.
//  Copyright © 2559 Lighting Bomb. All rights reserved.
//

#import <Foundation/Foundation.h>
#import  <Social/Social.h>
#import <MessageUI/MessageUI.h>

//typedef enum SocialApp{Facebook =0,Twitter =1, SinaWeibo=2,TencentWeibo=3}SocialApp;
@interface UnityShare : UIViewController<MFMailComposeViewControllerDelegate>{

@public
    CGRect PopoverRect;
}




-(void)Share:(char*)filesPathJson subject:(char*)subject message:(char*)message;

-(void)SocialShare:(char*)filesPathJson subject:(char*)subject message:(char*)message socialApp:(char*)socialApp;

-(void)SendEmail:(char*)subject message:(char*)message attachFilesJson:(char*)attachFilesJson emailsJson:(char*)emailsJson;
-(void)SendLine:(char*)filesPathJson message:(char*)message;

@end
