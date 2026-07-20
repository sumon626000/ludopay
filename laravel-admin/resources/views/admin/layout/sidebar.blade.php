<!-- BEGIN: Main Menu-->
    <div class="main-menu menu-fixed {{$websettings['sideskin_mode']}} menu-accordion menu-shadow" data-scroll-to-active="true">
      <div class="navbar-header">
        <ul class="nav navbar-nav flex-row">
          <li class="nav-item mr-auto"><a class="navbar-brand" href="{{url('/')}}/admin/dashboard"><span class="brand-logo">
              </span>
              <h2 class="brand-text">{{$websettings['website_name']}}</h2></a></li>
          <li class="nav-item nav-toggle"><a class="nav-link modern-nav-toggle pr-0" data-toggle="collapse"><i class="d-block d-xl-none text-primary toggle-icon font-medium-4" data-feather="x"></i><i class="d-none d-xl-block collapse-toggle-icon font-medium-4  text-primary" data-feather="disc" data-ticon="disc"></i></a></li>
        </ul>
      </div>
      <div class="shadow-bottom"></div>
      <div class="main-menu-content">
        <ul class="navigation navigation-main" id="main-menu-navigation" data-menu="menu-navigation">
         <li class=" nav-item"><a class="d-flex align-items-center" href="{{url('/')}}/admin/dashboard"><i class="las la-tachometer-alt"></i><span class="menu-title text-truncate" data-i18n="Email">Dashboard</span></a>
          </li>
           <li class=" nav-item"><a class="d-flex align-items-center" href="javascript:void(0);"><i class="las la-gamepad"></i><span class="menu-title text-truncate" data-i18n="Invoice">Player</span></a>
            <ul class="menu-content">
              <li><a class="d-flex align-items-center" href="{{url('/')}}/admin/player/all"><i class="las la-star"></i><span class="menu-item" data-i18n="List">All Player</span></a>
              </li>
              <li><a class="d-flex align-items-center" href="{{url('/')}}/admin/player/block"><i class="las la-star"></i><span class="menu-item" data-i18n="Preview">Block Player</span></a>
              </li>
            </ul>
          </li>
          
          <!-- <li class=" nav-item"><a class="d-flex align-items-center" href="{{url('/')}}/admin/special/offer"><i class="las la-percent"></i><span class="menu-title text-truncate" data-i18n="Invoice">Special Offer</span></a>
          </li> -->

          <li class=" nav-item"><a class="d-flex align-items-center" href="{{url('/')}}/admin/shop/coin"><i class="las la-shopping-cart"></i><span class="menu-title text-truncate" data-i18n="Invoice">Shop Coin</span></a>
          </li>

           <li class=" nav-item"><a class="d-flex align-items-center" href="{{url('/')}}/admin/bid/coin"><i class="las la-hand-holding-usd"></i><span class="menu-title text-truncate" data-i18n="Invoice">Bid Value</span></a>
          </li>

            <li class=" nav-item"><a class="d-flex align-items-center" href="javascript:void(0);"><i class="las la-id-card"></i><span class="menu-title text-truncate" data-i18n="Email">KYC Verification</span></a>
           <ul class="menu-content">
             <li><a class="d-flex align-items-center" href="{{url('/')}}/admin/pending/kyc"><i class="las la-star"></i><span class="menu-item" data-i18n="List">Pending KYC</span></a>
             </li>
             <li><a class="d-flex align-items-center" href="{{url('/')}}/admin/approved/kyc"><i class="las la-star"></i><span class="menu-item" data-i18n="Preview">Appeoved KYC</span></a>
             </li>
             <li><a class="d-flex align-items-center" href="{{url('/')}}/admin/rejected/kyc"><i class="las la-star"></i><span class="menu-item" data-i18n="Preview">Reject KYC</span></a>
             </li>
           </ul>
         </li>
         <li class=" nav-item"><a class="d-flex align-items-center"href="javascript:void(0);" ><i class="las la-file-invoice-dollar"></i><span class="menu-title text-truncate" data-i18n="Email">Withdraw Request</span></a>
            <ul class="menu-content">
              <li><a class="d-flex align-items-center" href="{{url('/')}}/admin/new/withdraw/request"><i class="las la-star"></i><span class="menu-item" data-i18n="List">New Request</span></a>
              </li>
              <li><a class="d-flex align-items-center" href="{{url('/')}}/admin/new/deposit/request"><i class="las la-star"></i><span class="menu-item" data-i18n="List">New Deposit Request</span></a>
              </li>
              <li><a class="d-flex align-items-center" href="{{url('/')}}/admin/approved/withdraw/request"><i class="las la-star"></i><span class="menu-item" data-i18n="Preview">Approved Withdraw</span></a>
              </li>
              <li><a class="d-flex align-items-center" href="{{url('/')}}/admin/rejected/withdraw/request"><i class="las la-star"></i><span class="menu-item" data-i18n="Preview">Rejected Withdraw</span></a>
              </li>
            </ul>
          </li>
           <li class=" nav-item"><a class="d-flex align-items-center" href="{{url('/')}}/admin/notification"><i class="las la-bell"></i><span class="menu-title text-truncate" data-i18n="Invoice">Notification</span></a>
          </li>
           <li class=" nav-item"><a class="d-flex align-items-center" href="javascript:void(0);"><i class="las la-money-check-alt"></i><span class="menu-title text-truncate" data-i18n="Invoice">Transactions</span></a>
            <ul class="menu-content">
              <li><a class="d-flex align-items-center" href="{{url('/')}}/admin/transction/all"><i class="las la-star"></i><span class="menu-item" data-i18n="List">All Transaction</span></a>
              </li>
              <li><a class="d-flex align-items-center" href="{{url('/')}}/admin/transction/success"><i class="las la-star"></i><span class="menu-item" data-i18n="Preview">Success Transaction</span></a>
              </li>
              <li><a class="d-flex align-items-center" href="{{url('/')}}/admin/transction/fail"><i class="las la-star"></i><span class="menu-item" data-i18n="Preview">Faield Transaction</span></a>
              </li>
            </ul>
          </li>
          <!-- <li class=" nav-item"><a class="d-flex align-items-center" href="javascript:void(0);"><i class="las la-flag"></i><span class="menu-title text-truncate" data-i18n="Email">Report</span></a>
          </li>
          <li class=" nav-item"><a class="d-flex align-items-center"href="javascript:void(0);" ><i class="las la-question-circle"></i><span class="menu-title text-truncate" data-i18n="Email">FAQ</span></a>
            <ul class="menu-content">
              <li><a class="d-flex align-items-center" href="{{url('/')}}/admin/faq/add"><i class="las la-star"></i><span class="menu-item" data-i18n="List">Add FAQ</span></a>
              </li>
              <li><a class="d-flex align-items-center" href="{{url('/')}}/admin/faq/all"><i class="las la-star"></i><span class="menu-item" data-i18n="Preview">Manage FAQ</span></a>
              </li>
            </ul>
          </li>
 -->
          <li class=" nav-item"><a class="d-flex align-items-center" href="{{url('/')}}/admin/websettings"><i class="las la-tools"></i><span class="menu-title text-truncate" data-i18n="Invoice">Website Settings</span></a></li>

          <li class=" nav-item"><a class="d-flex align-items-center" href="{{url('/')}}/admin/account"><i class="las la-user-cog"></i><span class="menu-title text-truncate" data-i18n="Invoice">Account Settings</span></a></li>
           <li class=" nav-item"><a class="d-flex align-items-center" href="logout"><i class="las la-sign-out-alt"></i><span class="menu-title text-truncate" data-i18n="Calendar">Logout</span></a>
          </li>
        </ul>
      </div>
    </div>
    <!-- END: Main Menu-->
