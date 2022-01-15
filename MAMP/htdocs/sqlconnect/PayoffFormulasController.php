<?php
namespace PayoffFormulasController;
include_once "CoreController.php";
use CoreController\core as core;

class payoffFormulas extends core
{
    public function create($data)
    {
		for($i = 1; $i <= $data["agentCount"]; $i++)
		{
			for($j = 1; $j <= $data["agentCount"]; $j++)
			{
				$agent1 = $data[$i."_".$j."_payoffFormula_agent1"];
				$agent2 = $data[$i."_".$j."_payoffFormula_agent2"];
				$payoffFormula = $data[$i."_".$j."_payoffFormula_payoffFormula"];
				$simulationID = $data[$i."_".$j."_payoffFormula_simulationID"];
				
				$SQL = "INSERT INTO payoffFormulas (agent1, agent2, payoffFormula, authorID) VALUES ($agent1, $agent2, \"$payoffFormula\", $simulationID);";
				
				$this -> con -> query ($SQL);
			}
		}
		echo "0;";
    }

    public function read($data)
    {		
		$simulationID = $data["simulationID"];
		$SQL = "SELECT * FROM payoffFormulas WHERE authorID = $simulationID;";
		$result = $this -> con -> query ($SQL) or die("sql failed!");
		echo '0;'.$result->num_rows;
		for($i = 0; $i < $result->num_rows; $i++)
		{
			$row = $result -> fetch_assoc();
			echo'{ID:'.$row["payoffFormulaID"].', agent1:'.$row["agent1"].', agent2:'.$row["agent2"].', payoffFormula:"'.$row["payoffFormula"].'", simulationID:'.$row["authorID"].'}';
		}
		
    }

    public function update($data)
    {
		$simulationID = $data["1_1_payoffFormula_simulationID"];
		
		$SQL = "DELETE FROM payoffFormulas WHERE authorID = $simulationID;";
		$this -> con -> query ($SQL);
		$i = 0; $j = 0;
		for($i = 1; $i <= $data["agentCount"]; $i++)
		{
			for($j = 1; $j <= $data["agentCount"]; $j++)
			{
				$agent1 = $data[$i."_".$j."_payoffFormula_agent1"];
				$agent2 = $data[$i."_".$j."_payoffFormula_agent2"];
				$payoffFormula = $data[$i."_".$j."_payoffFormula_payoffFormula"];
				$simulationID = $data[$i."_".$j."_payoffFormula_simulationID"];
				
				$SQL = "INSERT INTO payoffFormulas (agent1, agent2, payoffFormula, authorID) VALUES ($agent1, $agent2, \"$payoffFormula\", $simulationID);";
				
				$this -> con -> query ($SQL);
			}
		}
		$simulationID = $data["1_1_payoffFormula_simulationID"];
		echo "0;$simulationID";
    }

    public function delete($data)
    {
		
		$simulationID = $data["simulationID"];
		$SQL = "DELETE FROM payoffFormulas WHERE payoffFormulas.authorID = $simulationID;";
		$this -> con -> query ($SQL);
		echo "0;";
    }

}
?>