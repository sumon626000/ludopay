<?php

namespace App\Http\Controllers\Home;

use App\Http\Controllers\Controller;
use App\Models\Withdraw\Withdraw;
use App\Models\Transaction\Transaction;
use App\Models\Player\Userdata;
use App\Models\Kyc\Kycdetail;
use App\Models\Player\Roomdata;
use App\Models\WebSetting\Websetting;
use Illuminate\Support\Facades\DB;
use Illuminate\Http\Request;

class HomeController extends Controller
{
    public function Index(){
      // $Withdraw = DB::table('withdraws')->join('userdatas','withdraws.userid','userdatas.userid')->get();
       $Withdraw = Withdraw::latest()->paginate(10);
       $TopPlayer = Userdata::orderBy('winning_amount','desc')->first();
       if (!$TopPlayer) {
           $TopPlayer = (object) [
               'photo' => '',
               'username' => 'No players yet',
               'winning_amount' => 0,
               'points' => 0,
           ];
       }
       $Transaction = Transaction::latest()->paginate(10);
       $LiveMatch = Roomdata::count();
       $Userdata = Userdata::latest()->paginate(10);
       $totalUser = Userdata::count();
       $WalletAmount = Userdata::sum('points') ?? 0;
       $AllTransaction = Transaction::count();
       $TotalTransaction = Transaction::where("status","Success")->sum('amount');
       $TotalSentMoney = Withdraw::where("status","1")->sum('amount');
       //total withdraw request
       $TotalApprovedWithRequest = Withdraw::where("status","1")->count();
       $TotalPendingWithRequest = Withdraw::where("status","0")->count();
       $TotalRejectWithRequest = Withdraw::where("status","2")->count();
       //total kyc details
       $TotalApprovedKyc = Kycdetail::where("verification_status","1")->count();
       $TotalPendingKyc = Kycdetail::where("verification_status","0")->count();
       $TotalRejectKyc = Kycdetail::where("verification_status","2")->count();
       $websetting = Websetting::where('_id',"60bed6aef3c80e44a06e01f0")->first();
       if (!$websetting) {
           $websetting = Websetting::first();
       }
       if (!$websetting) {
           $websetting = (object) ['activeplayer' => 0];
       }

       $WinningAmount = Userdata::sum('winning_amount') ?? 0;
       $OwnerAmount = $WalletAmount-$WinningAmount;
       return view('admin.index',compact('Withdraw','Transaction','Userdata','totalUser','OwnerAmount','WinningAmount','WalletAmount','AllTransaction','TotalSentMoney','TotalApprovedWithRequest','TotalRejectWithRequest','TotalPendingWithRequest','TotalApprovedKyc','TotalPendingKyc','TotalRejectKyc','TopPlayer','websetting','LiveMatch'));
    }
}
