<?php

class SpecialXMLExport extends SpecialPage {

    function __construct() {
        parent::__construct('XMLExport');
    }

    function execute($par) {
        global $wgRequest, $wgOut;

        $this->setHeaders();

        # Get request data from, e.g.
        $exporter = $wgRequest->getText('exporter');


        if ($exporter == null)
            $this->displayVersions();
        else {
            $wgOut->addWikiText('Loading Exporters/' . $exporter . '.php');
            require_once 'Exporters/' . $exporter . '.php';

            $wgOut->addWikiText('Loading Game Objects');

            require_once 'GameData/Games.php';
            $games = new Games();

            $file = $wgRequest->getText('file_name');

            $games->loadFromDb($file);

            $exporter = new Exporter($file);
            $wgOut->addWikiText('<syntaxhighlight lang="xml" enclose="div">' . $exporter->export($games) . '</syntaxhighlight>');
       }
    }

    function displayVersions() {
        global $wgUser, $wgOut, $wgImportSources, $wgExportMaxLinkDepth;
        $wgOut->addHTML(
                Xml::openElement('form', array('enctype' => 'multipart/form-data', 'method' => 'post',
                    'action' => $action, 'id' => 'mw-import-upload-form')) .
                Xml::label('Exporter:', 'exporter'));


        $wgOut->addHTML('<select name="exporter">');

        $dbr = wfGetDB(DB_SLAVE);
        $res = $dbr->select(array('masgau_game_data.xml_versions'), //
                        '*', // $vars (columns of the table)
                        null, // $conds
                        __METHOD__, // $fname = 'Database::select',
                        array('ORDER BY' => 'minor ASC, major ASC')
                    );
        foreach($res as $row) {
            if($row->id!=0)
                $wgOut->addHTML('<option value="MASGAU-'.$row->string.'" selected="true">MASGAU-'.$row->string.'</option>');
        }
        
        $wgOut->addHTML('</select>');
        
        $wgOut->addHTML('<label for="platform">File:</label>');
        $wgOut->addHTML('<select name="file_name">');
        $res = $dbr->select(array('masgau_game_data.xml_files'), //
                        '*', // $vars (columns of the table)
                        null, // $conds
                        __METHOD__, // $fname = 'Database::select',
                        array('ORDER BY' => 'name ASC')
                    );
        foreach($res as $row) {
                $wgOut->addHTML('<option value="'.$row->name.'">'.$row->name.'</option>');
        }
        $wgOut->addHTML('</select>');

        $wgOut->addHTML(
                Xml::submitButton('Export') .
                Xml::closeElement('form') .
                Xml::closeElement('fieldset')
        );
    }

}