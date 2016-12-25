<?php

class Home extends Controller {

    function __construct() {
        parent::__construct();
    }
    
    public function index() {
        $this->view->title = "Home";
        $this->view->render('home/index');
    }

}