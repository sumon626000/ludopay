<?php

namespace App\Http\Controllers\Player;

use App\Http\Controllers\Controller;
use App\Models\Deposit\Deposit;
use App\Models\Player\Userdata;
use App\Models\Withdraw\Withdraw;
use App\Models\Transaction\Transaction;
use App\Models\Player\Gamehistory;
use App\Models\WebSetting\Websetting;
use Illuminate\Support\Facades\Crypt;
use Illuminate\Http\Request;

class PlayerController extends Controller
{
    public function AllPlayer(){
        $data = Userdata::latest()->paginate(10);
        return view('admin.Player.AllPlayer',compact('data'));
    }

    public function BlockPlayer(){
        $data = Userdata::where('banned',0)->latest()->paginate(10);
        return view('admin.Player.BlockedPlayer',compact('data'));
    }

    //view player details 

    public function ViewPlayerDetails($id){
        $data = Userdata::where('userid',Crypt::decrypt($id))->first();
        $TotalRefer =  Withdraw::where("userid",Crypt::decrypt($id))->where("userid",Crypt::decrypt($id))->count();
        $totalGamePlay = Gamehistory::where("userid",Crypt::decrypt($id))->count();
        $totalwinGamePlay = Gamehistory::where("userid",Crypt::decrypt($id))->where("game_status","win")->count();
        $totallossGamePlay = Gamehistory::where("userid",Crypt::decrypt($id))->where("game_status","loss")->count();
        $NoOfWithdraw =  Withdraw::where("userid",Crypt::decrypt($id))->count();
        $withdrawAmount = Withdraw::where("status","1")->where("userid",Crypt::decrypt($id))->sum('amount');
        $TotalTrans = Transaction::where("userid",Crypt::decrypt($id))->count();
        $TotalSuccessTrans = Transaction::where("status","Success")->where("userid",Crypt::decrypt($id))->count();
        $Websetting = Websetting::first();
        $TotalFailedTrans = Transaction::where("status","Failed")->where("userid",Crypt::decrypt($id))->count();
        return view('admin.Player.PlayerDetails',compact('data','NoOfWithdraw','withdrawAmount','TotalTrans','TotalSuccessTrans','TotalFailedTrans','Websetting','totalGamePlay','totalwinGamePlay','totallossGamePlay'));

    }


    public function AddCoin(Request $request){
        $UserData = Userdata::where('userid',$request->PlayerID)->first();
        $prevcoin = $UserData->points;
        $prevwincoin = $UserData->winning_amount;
        $TotalCoin = $prevcoin+$request->CoinValue;
        $TotalWinCoin = $prevwincoin+$request->WinValue;
        $response = Userdata::where("userid",$request->PlayerID)->update(array(
            "points" => $TotalCoin,
            "winning_amount" => $TotalWinCoin,
            ));

        //send response
          if($response){
            $request->session()->flash('success','Coin Added Successfully !');
            return back();
          }else{
            $request->session()->flash('error','Something Is Wrong Pleease Try Again !');
            return back();
          }

    }

     public function CutUserCoin(Request $request){
        $UserData = Userdata::where('userid',$request->PlayerID)->first();
        $prevcoin = $UserData->points;
        $prevwincoin = $UserData->winning_amount;
        $TotalCoin = $prevcoin-$request->CoinValue;
        $TotalWinCoin = $prevwincoin-$request->WinValue;
        $response = Userdata::where("userid",$request->PlayerID)->update(array(
            "points" => $TotalCoin,
            "winning_amount" => $TotalWinCoin,
            ));

        //send response
          if($response){
            $request->session()->flash('success','Coin Deduct Successfully !');
            return back();
          }else{
            $request->session()->flash('error','Something Is Wrong Pleease Try Again !');
            return back();
          }

    }

     public function UpdateUserDetails(Request $request){
        $response = Userdata::where("userid",$request->PlayerID)->update(array(
            "username" => $request->PlayerName,
            "userphone" => $request->PlayerPhone,
            "useremail" => $request->PlayerEmail,
            "points" => $request->TotalCoin,
            "winning_amount" => $request->TotalWinCoin,
            "kyc_status" => $request->KycStatus,
            ));

        //send response
          if($response){
            $request->session()->flash('success','User Data Updated Successfully !');
            return back();
          }else{
            $request->session()->flash('error','Something Is Wrong Pleease Try Again !');
            return back();
          }

    }

     public function UserBan(Request $request, $id){
        $response = Userdata::where("userid",$id)->update(array(
            "banned" => "0",
            ));
         if($response){
          return response(array("data"=>$response),200)->header("Content-Type","application/json");
         }
         else{
             return response(array("notice"=>"Data Not Delete"),404)->header("Content-Type","application/json");
         }

    }

      public function UserUnBan(Request $request, $id){
        $response = Userdata::where("userid",$id)->update(array(
            "banned" => "1",
            ));
         if($response){
          return response(array("data"=>$response),200)->header("Content-Type","application/json");
         }
         else{
             return response(array("notice"=>"Data Not Delete"),404)->header("Content-Type","application/json");
         }

    }

    //show transaction history

    public function TransctionHistory($id){
        $UserData = Userdata::where('userid',Crypt::decrypt($id))->first();
        $data = Transaction::where('userid',Crypt::decrypt($id))->latest()->paginate(10);
        return view('admin.Player.TransactionHistory',compact('data','UserData'));
    }

    public function WithdrawHistory($id){
        $UserData = Userdata::where('userid',Crypt::decrypt($id))->first();
        $data = Withdraw::where('userid',Crypt::decrypt($id))->latest()->paginate(10);
        return view('admin.Player.WithdrawHistory',compact('data','UserData'));
    }

     public function GameHistory($id){
        $UserData = Userdata::where('userid',Crypt::decrypt($id))->first();
        $data = Gamehistory::where('userid',Crypt::decrypt($id))->latest()->paginate(10);
        return view('admin.Player.GameHistory',compact('data','UserData'));
    }

     public function ReferdUser($id){
        $UserData = Userdata::where('userid',Crypt::decrypt($id))->first();
        $data = Userdata::where('used_refer_code',$UserData->referral_code)->latest()->paginate(10);
        return view('admin.Player.ReferdUser',compact('data','UserData'));
    }

    //now update withdraw status

    public function UpdateWithdrawStatus(Request $request){
        $response = Withdraw::where("_id",$request->RequestID)->update(array(
            "status" => $request->status,
            "transaction_id" => $request->transaction_id,
            ));

       // send response
          if($response){
            $request->session()->flash('success','Withdraw Status Updated Successfully !');
            return back();
          }else{
            $request->session()->flash('error','Something Is Wrong Pleease Try Again !');
            return back();
          }
    }
    public function UpdateDepositStatus(Request $request){
            // $request->session()->flash('success','Deposit Status Updated Successfully !' . $request->RequestID);
            // return back();

        $response = Deposit::where("id_dp",$request->RequestID )->where("username",$request->PlayerID)->update(array(
            "status_dp" => $request->status,
            "trxid" => $request->transaction_id,
            ));

       // send response
          if($response){
            $request->session()->flash('success','Deposit Status Updated Successfully !');
            return back();
          }else{
            $request->session()->flash('error','Something Is Wrong Pleease Try Again !');
            return back();
          }
    }

      public function DeletePlayer($id){
        $response = Userdata::find($id);
        $response = $response->delete();
        if($response){
          return response(array("notice"=>"Data Delete Success"),200)->header("Content-Type","application/json");
         }
         else{
             return response(array("notice"=>"Data Not Delete"),404)->header("Content-Type","application/json");
         }

      }

      //now search player 

      public function SearchPlayer(Request $request){
        $search = $request->searchInput;
        $data = Userdata::where('userid', 'LIKE', "%{$search}%")->orWhere('userphone', 'LIKE', "%{$search}%")->latest()->paginate(10);
      return view('admin.Player.search',compact('data'));

     }
}
