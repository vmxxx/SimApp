<?php
namespace LikesDislikesController;
include_once "CoreController.php";
use CoreController\core as core;

class likesDislikes extends core
{
    public function create($data)
    {
		$userID = $data["userID"];
		$simulationID = $data["simulationID"];
		$isLike = $data["isLike"];
		if($isLike == 1) $newLikesCount = $data["currLikesCount"] + 1;
		else $newDislikesCount = $data["currDislikesCount"] + 1;
		
        $SQL = "INSERT INTO likesdislikes (userID, simulationID, isLike) VALUES ($userID, $simulationID, $isLike);";
        $result = mysqli_query($this->con, $SQL);
		
		if ($isLike == 1) $SQL = "UPDATE simulations SET likesCount = $newLikesCount;";
		else $SQL = "UPDATE simulations SET dislikesCount = $newDislikesCount;";
        $result = mysqli_query($this->con, $SQL);

		echo "0; Like or dislike added successfully!";
    }

    public function read($data)
    {
		$userID = $data["userID"];
		$simulationID = $data["simulationID"];
        $SQL = "SELECT * FROM likesdislikes WHERE userID = $userID AND simulationID = $simulationID;";
        $result = mysqli_query($this->con, $SQL);

        if(mysqli_num_rows($result) > 0)
        {
			$row = $result -> fetch_assoc();
			$isLike = $row["isLike"];
			echo "0;$isLike";
        }
		else
		{
			echo "0; No like or dislike!";
		}
    }

    public function update($data)
    {
		$userID = $data["userID"];
		$simulationID = $data["simulationID"];
		$isLike = $data["isLike"];
		if ($isLike == 1) { $newLikesCount = $data["currLikesCount"] + 1; $newDislikesCount = $data["currDislikesCount"] - 1; }
		else { $newLikesCount = $data["currLikesCount"] - 1; $newDislikesCount = $data["currDislikesCount"] + 1; }
		
        $SQL = "UPDATE likesdislikes SET userID = $userID, simulationID = $simulationID, isLike = $isLike;";
        $result = mysqli_query($this->con, $SQL);
		
		$SQL = "UPDATE simulations SET likesCount = $newLikesCount, dislikesCount = $newDislikesCountikesCount;";
        $result = mysqli_query($this->con, $SQL);

		echo "0; Like or dislike added successfully!";
    }

    public function delete($data)
    {
		$userID = $data["userID"];
		$simulationID = $data["simulationID"];
		$isLike = $data["isLike"];
		if ($isLike == 1) $newLikesCount = $data["currLikesCount"] - 1;
		else $newDislikesCount = $data["currDislikesCount"] - 1;
		
        $SQL = "DELETE FROM likesdislikes WHERE userID = $userID AND simulationID = $simulationID;";
        $result = mysqli_query($this->con, $SQL);
		
		if ($isLike == 1) $SQL = "UPDATE simulations SET likesCount = $newLikesCount;";
		else $SQL = "UPDATE simulations SET dislikesCount = $newDislikesCount;";
        $result = mysqli_query($this->con, $SQL);

		echo "0; Like or dislike removed successfully!";
    }

}
?>