<!doctype html>
<html>
<head>
    <title><?=(isset($this->title)) ? "COMET | " . ucfirst($this->title) : 'COMET'; ?></title>
    <link rel="icon" href="<?= URL; ?>favicon.png" />
    
    <link rel="stylesheet" href="<?= URL; ?>public/css/bootstrap/bootstrap.min.css" />
    <link rel="stylesheet" href="<?= URL; ?>public/css/bootstrap/bootstrap-theme.min.css" />
    <link rel="stylesheet" href="<?= URL; ?>public/css/datatables/datatables.css" />
    <link rel="stylesheet" href="<?= URL; ?>public/css/datatables/datatablesCustomized.css" />
    
    <link rel="stylesheet" href="<?= URL; ?>public/css/header.css" />
    <link rel="stylesheet" href="<?= URL; ?>public/css/default.css" />
    <link rel="stylesheet" href="<?= URL; ?>public/css/api.css" />
    
    <script src="<?= URL; ?>public/js/jquery.js"></script>
    <script src="<?= URL; ?>public/js/bootstrap/bootstrap.min.js"></script>
    <script src="<?= URL; ?>public/js/datatables/datatables.js"></script>
    <script src="https://use.fontawesome.com/bcfab1323f.js"></script>
</head>
<body style="display: none;">

    <?php Session::init(); ?>
    
    <div id="header">
        <!-- Navigation here -->
    </div>

    <div id="content">
    
    