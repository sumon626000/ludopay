<?php

namespace App\Http\Controllers\PaymentGateway\Razorpay;

use App\Http\Controllers\Controller;
use Razorpay\Api\Api;
use Illuminate\Support\Str;
use Illuminate\Http\Request;

class RazorpayController extends Controller
{
    
    private $razorpayId = "rzp_test_yuNxWOc3Rr8Uc0";
    private $razorpayKey = "9fhQX5lfQeRp1iXwW6lsjjsu";


    public function Initiate()
    {
        // Let's see the documentation for creating the order

        // Generate random receipt id
        $receiptId = Str::random(10);

        $api = new Api($this->razorpayId, $this->razorpayKey);

        // In razorpay you have to convert rupees into paise we multiply by 100
        // Currency will be INR
        // Creating order
        $order = $api->order->create(
            array(
                'receipt' => $receiptId,
                'amount' => 500 * 100,
                'currency' => 'INR'
            )
        );

        // Let's return the response 

        // Let's create the razorpay payment page
        $response = [
            'orderId' => $order['id'],
            'razorpayId' => $this->razorpayId,
            'amount' => 500 * 100,
            'name' => "sdad",
            'currency' => 'INR',
            'email' => "chanan@gmail.com",
            'contactNumber' => "934538240",
            'address' => "village pusahio via bithan district samastipu",
            'description' => 'Demo payment',
        ];

        // Let's checkout payment page is it working
        return view('admin.Razorpay.PaymentInitate', compact('response'));
    }


    public function Complete(Request $request)
    {

        // print_r($request->all());


        // Now verify the signature is correct . We create the private function for verify the signature
        $signatureStatus = $this->SignatureVerify(
            $request->all()['rzp_signature'],
            $request->all()['rzp_paymentid'],
            $request->all()['rzp_orderid']
        );

        // If Signature status is true We will save the payment response in our database
        // In this tutorial we send the response to Success page if payment successfully made
        if ($signatureStatus == true) {
            return view('admin.Razorpay.PaymentSuccess');
        } else {
            return view('admin.Razorpay.PaymentFaield');
        }
    }

    // In this function we return boolean if signature is correct
    private function SignatureVerify($_signature, $_paymentId, $_orderId)
    {
        try {
            $api = new Api($this->razorpayId, $this->razorpayKey);
            $attributes  = array('razorpay_signature'  => $_signature,  'razorpay_payment_id'  => $_paymentId,  'razorpay_order_id' => $_orderId);
            $order  = $api->utility->verifyPaymentSignature($attributes);
            return true;
        } catch (\Exception $e) {
            // If Signature is not correct its give a excetption so we use try catch
            return false;
        }

}

}