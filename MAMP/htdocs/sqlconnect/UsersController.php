<?php
namespace UsersController;
include_once "CoreController.php";
use CoreController\core as core;

class users extends core
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
            //Add user to the table.
            //Hashing means that hacker can"t put in some information and get someone's password.
            //Salting means that they can't use look up the tables,
            //which is a way to quickly break through ecrypted (hashed) passwords and figure out what they are

            $salt = "\$5\$rounds=5000\$"."steamedhams".$username."\$"; //this is gonna run through 5000 rounds of shifting characters around and coming up with really long and garbled mess of code
            $hash = crypt($password, $salt);
            $insertuserquery = "INSERT INTO users (username, hash, salt) VALUES (\"$username\", \"$hash\", \"$salt\");";
            mysqli_query($this->con, $insertuserquery) or die("4; Insert user query failed");

            echo("0; Registration succesful");
        }
    }

    public function read($data)
    {
		
		
        $username = $data["username"];
        $password = $data["password"];

        //check if name exists
        $namecheckquery = 'SELECT * FROM users WHERE username = "'.$username.'";';


        $namecheck = mysqli_query($this->con, $namecheckquery) or die("2; Name check query failed");
        if (mysqli_num_rows($namecheck) != 1)
        {
            echo "5; Either no user with name, or more than one";
        }
        else
        {

            //get login info from query
            $existinginfo = mysqli_fetch_assoc($namecheck);
            $salt = $existinginfo["salt"];
            $hash = $existinginfo["hash"];

            $loginhash = crypt($password, $salt);
            if($hash != $loginhash)
            {
                echo "6; Incorrect password";
            }
            else
            {
                $result = $this -> con -> query ($namecheckquery);
                if ($result->num_rows == 1) {
                    echo "0; ".$existinginfo["username"];
                }
            }
        }
    }

    public function update($data)
    {

    }

    public function delete($data)
    {

    }
}



?>