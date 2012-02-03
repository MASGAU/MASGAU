<?php
class SpecialMASGAU extends SpecialPage {
        function __construct() {
                parent::__construct( 'MASGAU' );
        }
 
        function execute( $par ) {
                global $wgRequest, $wgOut;
 
                $this->setHeaders();
 
                # Get request data from, e.g.
         $param = $wgRequest->getText('param');
 
                # Do stuff
         # ...
         $output = 'Hello world!';
                $wgOut->addWikiText( $output );
        }
}