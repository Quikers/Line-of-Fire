<?php

class Database extends PDO
{
    
    public function __construct($DB_HOST, $DB_NAME, $DB_USER, $DB_PASS)
    {
        parent::__construct('mysql:host='.$DB_HOST.';dbname='.$DB_NAME, $DB_USER, $DB_PASS);
        $this->setAttribute(PDO::ATTR_DEFAULT_FETCH_MODE, PDO::FETCH_ASSOC);
    }
    
    public function Query($sql, $variables,$format = true, $returnLastInsertedID = false, $returnRowCount = false) {
        $stmt = $this->prepare($sql);
        $stmt->execute($variables);
        
        $result = array();
        foreach ($stmt as $row) {
            array_push($result, $row);
        }
        
        if (count($result) == 1 && $format == true) { $result = $result[0]; }
        
        if ($returnLastInsertedID) {
            $result["result"] = $result;
            $result["lastInsertedID"] = $this->lastInsertId();
        }
        
        if ($returnRowCount) { $result["rowCount"] = $stmt->rowCount(); }
        
        return $result;
    }
    
}