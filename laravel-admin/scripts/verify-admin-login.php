<?php

require __DIR__ . '/../vendor/autoload.php';

$app = require __DIR__ . '/../bootstrap/app.php';
$app->make(Illuminate\Contracts\Console\Kernel::class)->bootstrap();

use App\Models\Admin\Admin;
use Illuminate\Support\Facades\Hash;

$email = $argv[1] ?? 'admin@gmail.com';
$password = $argv[2] ?? 'NixSumon@Ludo2026';

$admin = Admin::where('email', $email)->first();
if (!$admin) {
    echo "NOT FOUND\n";
    exit(1);
}

echo "FOUND: {$admin->email}\n";
echo Hash::check($password, $admin->password) ? "PASSWORD OK\n" : "PASSWORD FAIL\n";
