<?php

    $con = new mysqli("localhost", "root", "root", "unityaccess");

    //check that connection happen
    if($con->connect_error)
    {
        echo "1; Connection failed";
    }
    else
    {
        $username = $_POST["username"];
        $password = $_POST["password"];

        //check if name exists
        $sql = 'SELECT ID, username, salt, hash FROM users WHERE username = "'.$username.'";';


//        $namecheck = mysqli_query($con, $namecheckquery) or die("2; Name check query failed");
//        if (mysqli_num_rows($namecheck) == 0)
//        {
//            echo "5; Either no user with name, or more than one";
//        }
//        else
//        {
//
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
                echo "0; Login succesful; ID: ".$ID;

                $result = $con -> query ($sql);
                if($result->num_rows > 0)
                {
                    //output data of each row
                    $row = $result->fetch_assoc();
                    while($row) {
                        echo '{ ID:'.$row['ID'].', username:"'.$row['username'].'" }';
                    }
                }
                else {
                    echo "0 results";
                }
            }
//        }
    }

?>