<?php

use App\Http\Controllers\Admin\AdminController;
use App\Http\Controllers\Account\AccountController;
use App\Http\Controllers\Faq\FaqController;
use App\Http\Controllers\Player\PlayerController;
use App\Http\Controllers\SpecialOffer\SpecialofferController;
use App\Http\Controllers\Shopcoin\ShopcoinController;
use App\Http\Controllers\Bidvalue\BidConteoller;
use App\Http\Controllers\Kyc\KycController;
use App\Http\Controllers\Home\HomeController;
use App\Http\Controllers\Websettings\WebSettingController;
use App\Http\Controllers\Notification\NotificationController;
use App\Http\Controllers\Notification\SingleNotificationController;
use Illuminate\Support\Facades\Route;

/*
|--------------------------------------------------------------------------
| Web Routes
|--------------------------------------------------------------------------
|
| Here is where you can register web routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| contains the "web" middleware group. Now create something great!
|
*/
use MongoDB\Client;
use App\Models\Admin\Admin;
use Illuminate\Support\Facades\Hash;
Route::get('/test-db', function () {
    // try {
    //     Admin::where('email', 'admin@gmail.com')
    //         ->update(['password' => Hash::make('admin')]);
    //     return "Password updated to 'admin'";
    // } catch(\Exception $e) {
    //     return "Error: " . $e->getMessage();
    // }


    try {
        $uri = config('database.connections.mongodb.dsn');
        $dbName = config('database.connections.mongodb.database', 'monsterg_monster');
        $client = new Client($uri);
        $db = $client->{$dbName};
        $collection = $db->admins;
        $result = $collection->find()->toArray();
        
        return response()->json([
            'status' => 'connected',
            'users' => $result
        ]);
    } catch (\Exception $e) {
        return response()->json([
            'status' => 'error',
            'message' => $e->getMessage()
        ], 500);
    }
});



Route::get('/admin',[AdminController::class,'index']);

Route::post('/admin/auth',[AdminController::class,'auth'])->name('admin.auth');

Route::group(['middleware'=>'admin_auth'],function(){
    Route::prefix('admin')->group(function(){
        //admin coding
    Route::get('/dashboard',[HomeController::class,'Index']);
    //logout route
    Route::get('/logout', function () {
        session()->forget('ADMIN_LOGIN');
        session()->forget('ADMIN_ID');
        session()->flash('error','Logout Successfully !');
        return view('admin.login.AdminLogin');
     });
     //product category controller


 //now account routing
    Route::get('/account',[AccountController::class,'index']);
    Route::post('/account/profile/general',[AccountController::class,'updateImage'])->name('update.profile.general');
    Route::post('/account/profile/password',[AccountController::class,'updatePassword'])->name('update.password');
    Route::post('/account/profile/information',[AccountController::class,'updateInfo'])->name('update.info');
    Route::post('/account/profile/socialmedia',[AccountController::class,'updateSocialMedia'])->name('update.socialmedia');

    //now PlayerController routing

    Route::get('/player/all',[PlayerController::class,'AllPlayer']);
    Route::get('/player/block',[PlayerController::class,'BlockPlayer']);
    Route::get('/player/view/{id}',[PlayerController::class,'ViewPlayerDetails']);
    Route::post('/player/add/coin',[PlayerController::class,'AddCoin'])->name('add.user.coin');
    Route::post('/player/cut/coin',[PlayerController::class,'CutUserCoin'])->name('cut.user.coin');
    Route::post('/player/details/update',[PlayerController::class,'UpdateUserDetails'])->name('update.user.details');
    Route::post('/player/ban/update/{player}',[PlayerController::class,'UserBan']);
    Route::post('/player/unban/update/{player}',[PlayerController::class,'UserUnBan']);
    Route::get('/player/transction/{id}',[PlayerController::class,'TransctionHistory']);
    Route::get('/player/withdraw/{id}',[PlayerController::class,'WithdrawHistory']);
    Route::get('/player/gamehistory/{id}',[PlayerController::class,'GameHistory']);
    Route::get('/player/referd/user/{id}',[PlayerController::class,'ReferdUser']);
    Route::post('/player/update/withdraw/status',[PlayerController::class,'UpdateWithdrawStatus'])->name('update.withdraw.status');
    Route::post('/player/update/deposit/status',[PlayerController::class,'UpdateDepositStatus'])->name('update.deposit.status');
    Route::post('/player/delete/{id}',[PlayerController::class,'DeletePlayer']);
    Route::get('/player/search/list',[PlayerController::class,'SearchPlayer'])->name('search.player.list');

    //now special offer routing

    //now create special offer
    Route::get('/special/offer',[SpecialofferController::class,'index'])->name('admin.SpecialOffer.SpecialOffer');
    Route::post('/special/offer/create',[SpecialofferController::class,'create'])->name('create.product.brand');
    Route::post('/special/offer/delete/{id}',[SpecialofferController::class,'delete']);
    Route::get('/special/offer/edit/{id}',[SpecialofferController::class,'edit']);
    Route::post('/special/offer/update/{id}',[SpecialofferController::class,'update']);

    //now create shop coin
    Route::get('/shop/coin',[ShopcoinController::class,'index'])->name('admin.Shopcoin.Shopcoin');
    Route::post('/shop/coin/create',[ShopcoinController::class,'create'])->name('create.shopcoin.new');
    Route::post('/shop/coin/delete/{id}',[ShopcoinController::class,'delete']);
    Route::get('/shop/coin/edit/{id}',[ShopcoinController::class,'edit']);
    Route::post('/shop/coin/update/{id}',[ShopcoinController::class,'update']);

     //now create bid coin value
    Route::get('/bid/coin',[BidConteoller::class,'index'])->name('admin.Bidvalue.Bidvalue');
    Route::post('/bid/coin/create',[BidConteoller::class,'create'])->name('create.bidvalue.new');
    Route::post('/bid/coin/delete/{id}',[BidConteoller::class,'delete']);
    Route::get('/bid/coin/edit/{id}',[BidConteoller::class,'edit']);
    Route::post('/bid/coin/update/{id}',[BidConteoller::class,'update']);

    //now faq routing
    Route::get('/faq/all',[FaqController::class,'index']);
    Route::get('/faq/add',[FaqController::class,'addFaqForm']);
    Route::post('faq/create',[FaqController::class,'FAQCreate'])->name('create.faq.new');
    Route::get('/faq/edit/{id}',[FaqController::class,'EditFaqForm']);
    Route::post('/faq/update/{id}',[FaqController::class,'UpdateFaq'])->name('update.faq');
    Route::post('/faq/delete/{id}',[FaqController::class,'DeleteFaq']);

    //web setting routing
    Route::get('/websettings',[WebSettingController::class,'index'])->name('admin.Websettings.websettings');
    Route::post('/websettings/general/update',[WebSettingController::class,'generalUpdate'])->name('update.general.setting');
    Route::post('/websettings/logo/update',[WebSettingController::class,'logoUpdate'])->name('update.logo');
    Route::post('/websettings/social/update',[WebSettingController::class,'socialUpdate'])->name('update.social');
    Route::post('/websettings/contact/update',[WebSettingController::class,'contactUpdate'])->name('update.contact');
    Route::post('/websettings/admin/about/update',[WebSettingController::class,'AdminAboutUpdate'])->name('update.Adminabout');
    Route::post('/websettings/payment/policy/update',[WebSettingController::class,'gameRule'])->name('update.GameRule');
    Route::post('/websettings/shipping/policy/update',[WebSettingController::class,'GameConfig'])->name('update.gameConfig');

    Route::post('/websettings/tearms/policy/update',[WebSettingController::class,'TermsPolicyUpdate'])->name('update.TermsPolicy');
    Route::post('/websettings/privacy/policy/update',[WebSettingController::class,'PrivacyPolicyUpdate'])->name('update.PrivacyPolicy');

    //now kyc routing

    Route::get('/pending/kyc',[KycController::class,'PendingKYC']);
    Route::get('/approved/kyc',[KycController::class,'ApprovedKYC']);
    Route::get('/rejected/kyc',[KycController::class,'RejectedKYC']);
    Route::get('/kyc/view/{id}',[KycController::class,'ViewKycDetails']);
    Route::post('/kyc/user/verified',[KycController::class,'VerifyKycStatus'])->name('verify.user.kyc');
    Route::post('/kyc/user/rejected',[KycController::class,'RejectKycStatus'])->name('reject.user.kyc');
    Route::post('/kyc/user/pending',[KycController::class,'PendingKycStatus'])->name('pending.user.kyc');

    //now withdraw routing
    
    Route::get('/new/withdraw/request',[KycController::class,'NewWithdrawRequest']);
    Route::get('/new/deposit/request',[KycController::class,'NewDepositRequest']);
    Route::get('/approved/withdraw/request',[KycController::class,'ApprovedWithdrawRequest']);
    Route::get('/rejected/withdraw/request',[KycController::class,'RejectedWithdrawRequest']);


    //now transction controller

    Route::get('/transction/all',[KycController::class,'AllTransaction']);
    Route::get('/transction/success',[KycController::class,'AllSuccessTransaction']);
    Route::get('/transction/fail',[KycController::class,'AllFailTransaction']);


    //Notification Routing

   Route::get('/notification',[NotificationController::class,'Index']);
   Route::post('/notification/send',[NotificationController::class,'send'])->name('send.all.notification');
   Route::post('/single/notification/send',[SingleNotificationController::class,'send'])->name('send.single.notofication');


    });
});


//now front routing

Route::get('payment/success',function(){
    return view("admin.Razorpay.PaymentSuccess");
});
Route::get('payment/failed',function(){
    return view("admin.Razorpay.PaymentFaield");
});


//Route::get('/',[FrontController::class,'index']);
