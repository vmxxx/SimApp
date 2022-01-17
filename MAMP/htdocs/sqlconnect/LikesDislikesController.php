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
		
        $SQL = "INSERT INTO likesdislikes (userID, simulationID, isLike) VALUES ($userID, $simulationID, $isLike);";
        $result = mysqli_query($this->con, $SQL);
		
		if ($isLike == 1) $SQL = "UPDATE simulations SET likesCount = likesCount + 1 WHERE ID = $simulationID;";
		else $SQL = "UPDATE simulations SET dislikeCount = dislikeCount + 1 WHERE ID = $simulationID;";
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
		
        $SQL = "UPDATE likesdislikes SET isLike = $isLike WHERE userID = $userID AND simulationID = $simulationID;";
        $result = mysqli_query($this->con, $SQL);
		
		if ($isLike == 1) { $SQL = "UPDATE simulations SET likesCount = likesCount + 1, dislikeCount = dislikeCount - 1 WHERE ID = $simulationID;"; }
		else { $SQL = "UPDATE simulations SET likesCount = likesCount - 1, dislikeCount = dislikeCount + 1 WHERE ID = $simulationID;"; }
        $result = mysqli_query($this->con, $SQL);

		echo "0; Like or dislike updated successfully!";
    }

    public function delete($data)
    {
		$userID = $data["userID"];
		$simulationID = $data["simulationID"];
		$wasLike = $data["wasLike"];
		
        $SQL = "DELETE FROM likesdislikes WHERE userID = $userID AND simulationID = $simulationID;";
        $result = mysqli_query($this->con, $SQL);
		
		if ($wasLike == 1) $SQL = "UPDATE simulations SET likesCount = likesCount - 1 WHERE ID = $simulationID;";
		else $SQL = "UPDATE simulations SET dislikeCount = dislikeCount - 1 WHERE ID = $simulationID;";
        $result = mysqli_query($this->con, $SQL);

		echo "0; Like or dislike removed successfully!";
    }

}
?>