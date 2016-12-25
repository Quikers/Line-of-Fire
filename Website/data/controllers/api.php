<?php

// API documentation starts

class User {

    function __construct() { }

    public $getuserbyuserid = "<p>Gets a <strong>user</strong> object from the database by searching for the <strong>user's</strong> <strong>userID</strong>.<br>You can request multiple <strong>users</strong> simultaneously by separating the <strong>userIDs</strong> with a comma. (1,2,3 etc.)<br><br>Usage:</p><pre>" . URL . "api/getuserbyuserid/<strong>:userid</strong>(,<strong>:userid</strong>)</pre><br><p>Examples:</p><pre>" . URL . "api/getuserbyuserid/1<br>" . URL . "api/getuserbyuserid/1,2</pre>";
    public $getuserbyemail = "<p>Gets a <strong>user</strong> object from the database by searching for the <strong>user's</strong> <strong>e-mail</strong>.<br>You can request multiple <strong>users</strong> simultaneously by separating the <strong>e-mails</strong> with a comma.<br><br>Usage:</p><pre>" . URL . "api/getuserbyemail/<strong>:e-mail</strong>(,<strong>:e-mail</strong>)</pre><br><p>Examples:</p><pre>" . URL . "api/getuserbyemail/admin@careforsometeasaboteur.com<br>" . URL . "api/getuserbyemail/admin@careforsometeasaboteur.com,test@test.test</pre>";
    public $getuserbyusername = "<p>Gets a <strong>user</strong> object from the database by searching for the <strong>user's</strong> <strong>username</strong>.<br>You can request multiple <strong>users</strong> simultaneously by separating the <strong>usernames</strong> with a comma.<br><br>Usage:</p><pre>" . URL . "api/getuserbyusername/<strong>:username</strong>(,<strong>:username</strong>)</pre><br><p>Examples:</p><pre>" . URL . "api/getuserbyusername/admin<br>" . URL . "api/getuserbyusername/admin,test</pre>";
    public $checklogin = "<p>Attempts to log in a <strong>user</strong> and if successful will send back the <strong>user's</strong> object.<br><br>Usage:</p><pre>" . URL . "api/checklogin/<strong>:e-mail</strong>/<strong>:password</strong></pre><br><p>Examples:</p><pre>" . URL . "api/checklogin/testemail@test.com/testpassword</pre>";
}

// API documentation ends

class API extends Controller {
    
    public $API;

    function __construct() {
        parent::__construct();
        
        $this->loadModel("API");
        $this->API = new APIModel();
    }
    
    public function index() {
        header("Location:" . URL . "api/docs");
    }
    
    public function docs($params = null) {
        $this->view->methodGroups = array("User");
        
        $this->view->title = "API Documentation";
        $this->view->render('api/index');
    }
    
    public function getuserbyuserid($params = null) {
        if (count($params) > 0) {
            echo json_encode($this->API->GetUserByUserID(explode(",", $params[0])));
        } else { echo 0; }
    }
    
    public function getuserbyemail($params = null) {
        if (count($params) > 0) {
            echo json_encode($this->API->GetUserByEmail(explode(",", $params[0])));
        } else { echo 0; }
    }
    
    public function getuserbyusername($params = null) {
        if (count($params) > 0) {
            echo json_encode($this->API->GetUserByUsername(explode(",", $params[0])));
        } else { echo 0; }
    }
    
    public function checklogin($params = null) {
        if (count($params) == 2) {
            echo json_encode($this->API->CheckLogin($params[0], $params[1]));
        } else { echo 0; }
    }

}