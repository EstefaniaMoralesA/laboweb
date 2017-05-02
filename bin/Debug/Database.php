<?php
class MySQL{
	
	/* Variables de BDD */
	private $dbname = "@dbname";
	private $dbuser = "@dbuser";
	private $dbpssw = "@dbpassword";
	private $dbsrvr = "@dbserver";
	
	/* Variables de buffer */
	private $resultSet;
	private $query;
	private $connection;
	
	/* Variables de utilería */
	private $wc = '/1QQ/';

	public function MySQL(){
		$this->connection =
		mysqli_connect($this->dbsrvr,$this->dbuser,$this->dbpssw,$this->dbname);

		// Check connection
		if (mysqli_connect_errno())
		  {
		  echo "Failed to connect to MySQL: " . mysqli_connect_error();
		  }
	}
	
	public function query($q = "eempty"){
		if($q == "eempty")
			$q = $this->query;
		$this->resultSet = mysqli_query($this->connection,$q);
	}
	
	public function fetch($rS = "eempty"){
		if($rS == "eempty")
			$rS = $this->resultSet;
		if(!$this->resultSet)
		{
			echo("Error description: " . mysqli_error($this->connection));
			exit();
		}
		return mysqli_fetch_array($rS);
	}
	
	public function qarray($q = "eempty", $arr = array()){
		$this->squery($q,$arr);
		return $this->fetch();
	}
	
	public function qvalue($q = "eempty", $arr = array()){
		return $this->qarray($q,$arr)[0];
	}
	
	public function rows($rS = "eempty"){
		if($rS == "eempty")
			$rS = $this->resultSet;
		return mysqli_num_rows($rS);
	}
	
	public function secure($str){
		$str = preg_replace('/</',"&lt;",$str);
		$str = preg_replace('/>/',"&gt;",$str);
		return mysqli_real_escape_string($this->connection,$str);
	}
	
	public function secure_string($str,$arr){
		foreach($arr as $a){
			$aa = $this->secure($a);
			$str = preg_replace($this->wc,$aa,$str,1);
		}
		return $str;
	}

	public function squery_rows($q = "eempty", $arr = array()){
		$this->squery($q, $arr);
		return $this->resultSet;
	}
	
	public function squery($q = "eempty", $arr = array()){
		if(count($arr) == 0)
			$this->query($q);
		else
			$this->query($this->secure_string($q,$arr));
	}
}
?>