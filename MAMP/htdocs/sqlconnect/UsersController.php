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

        $SQL = "SELECT username FROM users WHERE username = \"$username\";";
        //check if name exists
        $result = mysqli_query($this->con, $SQL);

        if(mysqli_num_rows($result) > 0)
        {
            echo "Such username already exists!";
        }
        else
        {
            //Add user to the table.
            //Hashing means that hacker can"t put in some information and get someone's password.
            //Salting means that they can't use look up the tables,
            //which is a way to quickly break through ecrypted (hashed) passwords and figure out what they are

            $salt = "\$5\$rounds=5000\$"."steamedhams".$username."\$"; //this is gonna run through 5000 rounds of shifting characters around and coming up with really long and garbled mess of code
            $hash = crypt($password, $salt);
            $SQL = "INSERT INTO users (username, hash, salt, isAdmin) VALUES (\"$username\", \"$hash\", \"$salt\", 0);";
            mysqli_query($this->con, $SQL);

            echo "0; Registration succesful!";
        }
    }

    public function read($data)
    {
		
		
        $username = $data["username"];
        $password = $data["password"];

        //check if name exists
        $SQL = "SELECT * FROM users WHERE username = \"$username\";";
		$result = $this -> con -> query ($SQL);
		
        if (mysqli_num_rows($result) != 1)
        {
            echo "Wrong username or password!";
        }
        else
        {

            //get login info from query
            $existinginfo = mysqli_fetch_assoc($result);
            $salt = $existinginfo["salt"];
            $hash = $existinginfo["hash"];

            $loginhash = crypt($password, $salt);
            if($hash != $loginhash)
            {
                echo "Wrong username or password!";
            }
            else
            {
				$ID = $existinginfo["ID"];
				$username = $existinginfo["username"];
				$isAdmin = $existinginfo["isAdmin"];
				
				echo "0; {ID:$ID, username:\"$username\", isAdmin:$isAdmin}";
            }
		}
    }

    public function update($data)
    {
		$ID = $data["ID"];
        $username = $data["username"];
        $password = $data["password"];
		
		
        $SQL = "SELECT username FROM users WHERE username = \"$username\";";
        //check if name exists
        $result = mysqli_query($this->con, $SQL);

        if(mysqli_num_rows($result) > 0)
        {
            echo "Such username already exists!";
        }
		else
		{
			//Add user to the table.
			//Hashing means that hacker can"t put in some information and get someone's password.
			//Salting means that they can't use look up the tables,
			//which is a way to quickly break through ecrypted (hashed) passwords and figure out what they are

			$salt = "\$5\$rounds=5000\$"."steamedhams".$username."\$"; //this is gonna run through 5000 rounds of shifting characters around and coming up with really long and garbled mess of code
			$hash = crypt($password, $salt);
			$SQL = "UPDATE users SET username = \"$username\", hash = \"$hash\", salt = \"$salt\" WHERE ID = $ID;";
			mysqli_query($this->con, $SQL);

			echo "0; Update succesful";
		}
    }

    public function delete($data)
    {

    }
}



?>