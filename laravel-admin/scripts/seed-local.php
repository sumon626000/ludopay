<?php

require __DIR__ . '/../vendor/autoload.php';

$app = require __DIR__ . '/../bootstrap/app.php';
$app->make(Illuminate\Contracts\Console\Kernel::class)->bootstrap();

use App\Models\Admin\Admin;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Hash;

$email = 'admin@gmail.com';
$password = 'NixSumon@Ludo2026';

$exists = DB::table('admins')->where('email', $email)->exists();
DB::table('admins')->updateOrInsert(
    ['email' => $email],
    [
        'name' => 'Admin',
        'username' => 'admin',
        'role' => 'admin',
        'password' => Hash::make($password),
    ]
);

echo $exists ? "Admin password updated\n" : "Admin user created\n";
