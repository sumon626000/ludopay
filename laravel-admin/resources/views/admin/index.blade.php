@extends('admin.layout.master')
@section('title')
 Admin Dashboard
@endsection
@section('css')
 <!--  link custom css link here -->
 <link rel="stylesheet" type="text/css" href="{{URL::asset('admin-assets/css/plugins/charts/chart-apex.min.css')}}">
 <link rel="stylesheet" type="text/css" href="{{URL::asset('admin-assets/vendors/css/charts/apexcharts.css')}}">
@endsection
@section('content')
 <!-- Dashboard Ecommerce Starts -->
<section id="dashboard-analytics">
  <div class="row match-height">
    <!-- Greetings Card starts -->
    <div class="col-lg-3 col-md-12 col-sm-12">
      <div class="card card-congratulations">
        <div class="card-body text-center">
          <img src="{{url('/')}}/admin-assets/images/elements/decore-left.png" class="congratulations-img-left" alt="card-img-left"/>
          <img src="{{url('/')}}/admin-assets/images/elements/decore-right.png"
            class="congratulations-img-right" alt="card-img-right"/>
          <div class="avatar avatar-xl bg-primary shadow">
            <div class="avatar-content">
              @if($TopPlayer->photo == "")
                 <img src="https://xp.io/storage/1QhD9ia1.png" width="100">
              @else
                <img src="{{$TopPlayer->photo}}" width="100">
              @endif
            </div>
          </div>
          <div class="text-center">
            <h1 class="mb-1 text-white"><b>Top Player</b></h1>
            <p class="card-text m-auto w-85">
             {{$TopPlayer->username}}<br>
             Winning Amount - {{$TopPlayer->winning_amount}}<br>
             Current Amount - {{$TopPlayer->points}}
            </p>
          </div>
        </div>
      </div>
    </div>
    <!-- Greetings Card ends -->
    <div class="col-lg-3 col-sm-6 col-12">
        <div class="card">
        <div class="card-header">
          <div>
            <h2 class="font-weight-bolder mb-0">{{ $LiveMatch }}</h2>
            <p class="card-text">Total Live Match </p>
          </div>
          <div class="avatar bg-danger p-50 m-0">
            <div class="avatar-content">
              <i class="las la-dot-circle font-medium-5"></i>
            </div>
          </div>
        </div>
      </div>
        <div class="card">
        <div class="card-header">
          <div>
            <h2 class="font-weight-bolder mb-0">{{$websetting->activeplayer}}</h2>
            <p class="card-text">Total Active Player </p>
          </div>
          <div class="avatar bg-success p-50 m-0">
            <div class="avatar-content">
              <i class="las la-dot-circle font-medium-5"></i>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- Subscribers Chart Card starts -->
      <div class="col-lg-3 col-sm-6 col-12">
      <div class="card">
        <div class="card-header flex-column align-items-start pb-0">
          <div class="avatar bg-info p-50 m-0">
            <div class="avatar-content">
              <i class="las la-users font-medium-5"></i>
            </div>
          </div>
          <h2 class="font-weight-bolder mt-1">
            @if($totalUser > 1000)
            {{$totalUser/1000}} K
            @else
            {{$totalUser}}
            @endif
          </h2>
          <p class="card-text">Total Register Players</p>
        </div>
        <div id="line-area-chart-5"></div>
      </div>
    </div>
    <!-- Subscribers Chart Card ends -->

    <!-- Orders Chart Card starts -->
    <div class="col-lg-3 col-sm-6 col-12">
      <div class="card">
        <div class="card-header flex-column align-items-start pb-0">
          <div class="avatar bg-warning p-50 m-0">
            <div class="avatar-content">
              <i class="las la-rupee-sign font-medium-5"></i>
            </div>
          </div>
          <h2 class="font-weight-bolder mt-1">
          @if($OwnerAmount > 1000)
          {{$OwnerAmount/1000}} K
          @else
          {{$OwnerAmount}}
          @endif
           <i class="las la-rupee-sign"></i></h2>
          <p class="card-text">Owner Approx Income</p>
        </div>
         <div id="line-area-chart-7"></div>
      </div>
    </div>
    <!-- Orders Chart Card ends -->
  </div>

  <!-- Line Area Chart Card -->
  <div class="row">
    <div class="col-lg-3 col-sm-6 col-12">
      <div class="card">
        <div class="card-header flex-column align-items-start pb-0">
          <div class="avatar bg-primary p-50 m-0">
            <div class="avatar-content">
             <i class="las la-rupee-sign font-medium-5"></i>
            </div>
          </div>
          <h2 class="font-weight-bolder mt-1">
            @if($WinningAmount > 1000)
            {{$WinningAmount/1000}} K
            @else
            {{$WinningAmount}}
            @endif
           <i class="las la-rupee-sign"></i></h2>
          <p class="card-text">Total Winning Amount</p>
        </div>
        <div id="line-area-chart-1"></div>
      </div>
    </div>
    <div class="col-lg-3 col-sm-6 col-12">
      <div class="card">
        <div class="card-header flex-column align-items-start pb-0">
          <div class="avatar bg-success p-50 m-0">
            <div class="avatar-content">
              <i class="las la-rupee-sign font-medium-5"></i>
            </div>
          </div>
          <h2 class="font-weight-bolder mt-1">
            @if($WalletAmount > 1000)
            {{$WalletAmount/1000}} K
            @else
            {{$WalletAmount}}
            @endif

            <i class="las la-rupee-sign"></i></h2>
          <p class="card-text">Total Wallet Amount</p>
        </div>
        <div id="line-area-chart-2"></div>
      </div>
    </div>
    <div class="col-lg-3 col-sm-6 col-12">
      <div class="card">
        <div class="card-header flex-column align-items-start pb-0">
          <div class="avatar bg-warning p-50 m-0">
            <div class="avatar-content">
              <i class="las la-exchange-alt font-medium-5"></i>
            </div>
          </div>
          <h2 class="font-weight-bolder mt-1">
            @if($AllTransaction > 1000)
            {{$AllTransaction/1000}} K
            @else
            {{$AllTransaction}}
            @endif
          </h2>
          <p class="card-text">Total Transaction</p>
        </div>
        <div id="line-area-chart-3"></div>
      </div>
    </div>

    <div class="col-lg-3 col-sm-6 col-12">
      <div class="card">
        <div class="card-header flex-column align-items-start pb-0">
          <div class="avatar bg-success p-50 m-0">
            <div class="avatar-content">
              <i class="las la-paper-plane font-medium-5"></i>
            </div>
          </div>
          <h2 class="font-weight-bolder mt-1">
            @if($TotalSentMoney > 1000)
            {{$TotalSentMoney/1000}} K
            @else
            {{$TotalSentMoney}}
            @endif
           <i class="las la-rupee-sign"></i></h2>
          <p class="card-text">Total Amount Sent</p>
        </div>
        <div id="line-area-chart-4"></div>
      </div>
    </div>
  </div>
  <!--/ Line Area Chart Card -->
    <!-- Line Chart Card -->
  <div class="row">
    <div class="col-lg-4 col-sm-6 col-12">
      <div class="card">
        <div class="card-header align-items-start pb-0">
          <div>
            <h2 class="font-weight-bolder">
            @if($TotalPendingWithRequest > 1000)
            {{$TotalPendingWithRequest/1000}} K
            @else
            {{$TotalPendingWithRequest}}
            @endif
            </h2>
            <p class="card-text mb-2">Total Pending Withdraw Request</p>
          </div>
          <div class="avatar bg-warning p-50">
            <div class="avatar-content">
             <i class="las la-hand-holding-usd font-medium-5"></i>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="col-lg-4 col-sm-6 col-12">
      <div class="card">
        <div class="card-header align-items-start pb-0">
          <div>
            <h2 class="font-weight-bolder">
            @if($TotalApprovedWithRequest > 1000)
            {{$TotalApprovedWithRequest/1000}} K
            @else
            {{$TotalApprovedWithRequest}}
            @endif
            </h2>
            <p class="card-text mb-2">Total Approved Withdraw Request</p>
          </div>
          <div class="avatar bg-success p-50">
            <div class="avatar-content">
             <i class="las la-check-circle font-medium-5"></i>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="col-lg-4 col-sm-6 col-12">
      <div class="card">
        <div class="card-header align-items-start pb-0">
          <div>
            <h2 class="font-weight-bolder">
            @if($TotalRejectWithRequest > 1000)
            {{$TotalRejectWithRequest/1000}} K
            @else
            {{$TotalRejectWithRequest}}
            @endif
            </h2>
            <p class="card-text mb-2">Total Reject Withdraw Request</p>
          </div>
          <div class="avatar bg-danger p-50">
            <div class="avatar-content">
             <i class="las la-times-circle font-medium-5"></i>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  <!--/ Line Chart Card -->
   <!-- Line Chart Card -->
  <div class="row">
    <div class="col-lg-4 col-sm-6 col-12">
      <div class="card">
        <div class="card-header align-items-start pb-0">
          <div>
            <h2 class="font-weight-bolder">
            @if($TotalPendingKyc > 1000)
            {{$TotalPendingKyc/1000}} K
            @else
            {{$TotalPendingKyc}}
            @endif
            </h2>
            <p class="card-text mb-2">Total Pending KYC</p>
          </div>
          <div class="avatar bg-warning p-50">
            <div class="avatar-content">
             <i class="las la-fingerprint font-medium-5"></i>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="col-lg-4 col-sm-6 col-12">
      <div class="card">
        <div class="card-header align-items-start pb-0">
          <div>
            <h2 class="font-weight-bolder">
            @if($TotalApprovedKyc > 1000)
            {{$TotalApprovedKyc/1000}} K
            @else
            {{$TotalApprovedKyc}}
            @endif
            </h2>
            <p class="card-text mb-2">Total Approved KYC</p>
          </div>
          <div class="avatar bg-success p-50">
            <div class="avatar-content">
             <i class="las la-fingerprint font-medium-5"></i>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="col-lg-4 col-sm-6 col-12">
      <div class="card">
        <div class="card-header align-items-start pb-0">
          <div>
            <h2 class="font-weight-bolder">
            @if($TotalRejectKyc > 1000)
            {{$TotalRejectKyc/1000}} K
            @else
            {{$TotalRejectKyc}}
            @endif
            </h2>
            <p class="card-text mb-2">Total Reject KYC</p>
          </div>
          <div class="avatar bg-danger p-50">
            <div class="avatar-content">
             <i class="las la-fingerprint font-medium-5"></i>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  <!--/ Line Chart Card -->
  

  <div class="row match-height">
    <!-- Timeline Card -->
    <div class="col-lg-4 col-12">
      <div class="card">
      <div class="card-header">
        <p class="card-title"><i class="las la-users"></i> Recent New Registration</p>
      </div>
      <div class="table-responsive">
        <table class="table">
          <thead>
            <tr>
              <th>#</th>
              <th>User ID</th>
              <th>Name</th>
              <th>Images</th>
            </tr>
          </thead>
          <tbody>
            @foreach($Userdata as $key =>$result)
            <tr>
              <td><span class="font-weight-bold">{{ $Userdata->firstItem() + $key }}</span></td>
              <td>{{$result->userid}}</td>
              <td>{{$result->username}}</td>
              <td><img src="$result->photo" width="50"></td>
            </tr>
            @endforeach
          </tbody>
        </table>
      </div>
    </div>
    </div>
    <!--/ Timeline Card -->
    <!-- Timeline Card -->
    <div class="col-lg-4 col-12">
      <div class="card">
      <div class="card-header">
         <p class="card-title"><i class="las la-exchange-alt"></i> Recent Transaction</p>
      </div>
      <div class="table-responsive">
        <table class="table">
          <thead>
            <tr>
              <th>#</th>
              <th>User ID</th>
              <th>Amount</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            @foreach($Transaction as $key =>$result)
            <tr>
              <td><span class="font-weight-bold">{{ $Transaction->firstItem() + $key }}</span></td>
              <td>{{$result->userid}}</td>
              <td><b>{{$result->amount}} INR</b></td>
              <td>
                @if($result->status == "Success")
              <div class="badge badge-light-success">Success</div>
                @else
              <div class="badge badge-light-danger">Failed</div>
                @endif

              </td>
            </tr>
            @endforeach
          </tbody>
        </table>
      </div>
    </div>
    </div>
    <!--/ Timeline Card -->
    <!-- Timeline Card -->
    <div class="col-lg-4 col-12">
      <div class="card">
      <div class="card-header">
        <p class="card-title"><i class="las la-hand-holding-usd"></i> Recent Withdraw</p>
      </div>
      <div class="table-responsive">
        <table class="table">
          <thead>
            <tr>
              <th>#</th>
              <th>User ID</th>
              <th>Amount</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            @foreach($Withdraw as $key =>$result)
            <tr>
              <td><span class="font-weight-bold">{{ $Withdraw->firstItem() + $key }}</span></td>
              <td>{{$result->userid}}</td>
              <td><b>{{$result->amount}} INR</b></td>
              <td>
                @if($result->status == 1)
              <div class="badge badge-light-success">Success</div>
                @else
              <div class="badge badge-light-danger">Failed</div>
                @endif

              </td>
            </tr>
            @endforeach
          </tbody>
        </table>
      </div>
    </div>
    </div>
    <!--/ Timeline Card -->


 
    <!--/ Timeline Card -->
  </div>

</section>
<!-- Dashboard Ecommerce ends -->
@endsection
@section('js')
 <script src="{{URL::asset('admin-assets/vendors/js/charts/apexcharts.min.js')}}"></script>
  <script src="{{URL::asset('admin-assets/js/scripts/cards/card-statistics.min.js')}}"></script>
<!-- link custom js link here -->
@endsection