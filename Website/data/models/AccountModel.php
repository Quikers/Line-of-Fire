<?php

class AccountModel extends Model {
    
    function __construct() {
        parent::__construct();
    }
    
    public function Login($username, $password) {
        $result = $this->db->Query(
            'SELECT `id`, `username`, `account_type`, `email`, `created`, `editted` FROM users WHERE `username` = :username AND `password` = PASSWORD(:password)',
            array(
                "username" => $username,
                "password" => $password
            )
        );
        
        if ($result != array()) {
            return $result;
        } else {
            return false;
        }
    }
    
    public function GetLastInsertedUser() {
        return $this->db->Query("SELECT (`id`) FROM `users` ORDER BY `id` DESC LIMIT 1");
    }
    
    public function Register($username, $password, $email) {
        try {
            $lastID = $this->GetLastInsertedUser()["id"];
            $this->db->Query(
                'INSERT INTO `users`(`username`, `password`, `account_type`, `email`, `activated`) VALUES (:username, PASSWORD(:password), 3, :email, 1)',
                array(
                    "username" => $username,
                    "password" => $password,
                    "email" => $email
                ), true, false, true
            );
            $newID = $this->GetLastInsertedUser()["id"];
        } catch(Exception $ex) { return -1; }
        
        return $lastID != $newID ? $newID : 0;
    }

}