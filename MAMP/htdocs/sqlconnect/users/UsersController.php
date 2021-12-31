<?php
namespace UsersController;
include "CoreController.php";
use CoreController\core as C;



class users extends C
{
    public function create($data)
    {
        $username = $data["username"];
        $password = $data["password"];

        $sql = "SELECT username FROM users WHERE username = \"$username\";";
        //check if name exists
        $namecheck = mysqli_query($this->con, $sql) or die("2; Name check query failed");

        if(mysqli_num_rows($namecheck) > 0)
        {
            echo "3; Name already exists";
            exit();
        }
        else
        {
            //add user to the table

            //hashing means that hacker can"t put in some information and get someone's password

            //salting means that they can't use look up the tables,
            //which is a way to quickly break through ecrypted (hashed) passwords and figure out what they are

            $salt = "\$5\$rounds=5000\$"."steamedhams".$username."\$"; //this is gonna run through 5000 rounds of shifting characters around and coming up with really long and garbled mess of code
            $hash = crypt($password, $salt);
            $insertuserquery = "INSERT INTO users (username, hash, salt) VALUES (\"$username\", \"$hash\", \"$salt\");";
            mysqli_query($this->con, $insertuserquery) or die("4; Insert user query failed");

            echo("0; Registration succesful");
        }
    }
}



?>