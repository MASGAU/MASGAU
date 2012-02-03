<?php

class SpecialXMLImport extends SpecialPage {

    function __construct() {
        parent::__construct('XMLImport');
    }

    function execute($par) {
        global $wgRequest, $wgUser, $wgOut;

        $this->setHeaders();

        if (wfReadOnly()) {
            $wgOut->readOnlyPage();
            return;
        }

        if (!$wgUser->isAllowedAny('import', 'importupload')) {
            return $wgOut->permissionRequired('import');
        }
        # @todo Allow Title::getUserPermissionsErrors() to take an array
        # @todo FIXME: Title::checkSpecialsAndNSPermissions() has a very wierd expectation of what
        # getUserPermissionsErrors() might actually be used for, hence the 'ns-specialprotected'
        $errors = wfMergeErrorArrays(
                $this->getTitle()->getUserPermissionsErrors(
                        'import', $wgUser, true, array('ns-specialprotected', 'badaccess-group0', 'badaccess-groups')
                ), $this->getTitle()->getUserPermissionsErrors(
                        'importupload', $wgUser, true, array('ns-specialprotected', 'badaccess-group0', 'badaccess-groups')
                )
        );

        if ($errors) {
            $wgOut->showPermissionsErrorPage($errors);
            return;
        }

        if ($wgRequest->wasPosted()) {
            switch($wgRequest->getVal('action')) {
                case 'submit':
                    $this->doImport();
                    break;
                case 'update':
                    $this->doUpdate();
                    break;
                default:
                    $this->showForm();
                    break;
            }
        } else {
            $this->showForm();
        }
    }

    private function doUpdate() {
        global $wgRequest, $wgUser, $wgOut;
        $files = $wgRequest->getArray('files');
        
        $date = new DateTime();
        foreach($files as $file) {
            $dbr = wfGetDB(DB_MASTER);
            $conds = array('last_updated' => $date->format('Y-m-d H:i:s'));

            $dbr->update('masgau_game_data.xml_files', $conds, array('name = \'' . $file . '\''), $fname = 'DatabaseBase::update', null);
            $wgOut->addWikiText("Updated ".$file);
        }
        $this->showForm();
    }
    
    private function doImport() {
        global $wgOut, $wgRequest, $wgUser, $wgImportSources, $wgExportMaxLinkDepth;

        require_once('GameData/Games.php');
        $games = new Games();
        $games->loadFromXml($_FILES["xmlimport"]["tmp_name"]);

        if ($wgRequest->getVal('overwrite') != null)
            $games->writeToDb(true);
        else
            $games->writeToDb(false);
    }

    private function showForm() {
        global $wgUser, $wgOut, $wgImportSources, $wgExportMaxLinkDepth;

        $action = $this->getTitle()->getLocalUrl(array('action' => 'submit'));

        if ($wgUser->isAllowed('importupload')) {
            $wgOut->addHTML(
                    Xml::openElement('form', array('enctype' => 'multipart/form-data', 'method' => 'post',
                        'action' => $action, 'id' => 'mw-import-upload-form')) .
                    Html::hidden('action', 'submit') .
                    Html::hidden('source', 'upload') .
                    Xml::openElement('table', array('id' => 'mw-import-table')) .
                    "<tr>
					<td class='mw-label'>" .
                    Xml::label(wfMsg('import-upload-filename'), 'xmlimport') .
                    "</td>
					<td class='mw-input'>" .
                    Xml::input('xmlimport', 50, '', array('type' => 'file')) . ' ' .
                    "</td>
				</tr>
				<tr>
					<td><input name='overwrite' type='checkbox' />Overwrite Existing</td>
					<td class='mw-submit'>" .
                    Xml::submitButton(wfMsg('uploadbtn')) .
                    "</td>
				</tr>" .
                    Xml::closeElement('table') .
                    Html::hidden('editToken', $wgUser->editToken()) .
                    Xml::closeElement('form') .
                    Xml::closeElement('fieldset')
            );
            
                    $dbr = wfGetDB(DB_SLAVE);

        $wgOut->addHTML(
                Xml::openElement('form', array('enctype' => 'multipart/form-data', 'method' => 'post',
                    'id' => 'mw-import-upload-form')) .
                Html::hidden('action', 'update'));
            $wgOut->addHTML('<label for="platform">Mark file(s) as updated:</label><br />');
            $res = $dbr->select(array('masgau_game_data.xml_files'), //
                    '*', // $vars (columns of the table)
                    null, // $conds
                    __METHOD__, // $fname = 'Database::select',
                    array('ORDER BY' => 'name ASC')
            );
            foreach ($res as $row) {
                $wgOut->addHTML('<input type="checkbox" name="files[]" value="'.$row->name.'" />' . $row->name . ' (Last Updated '.$row->last_updated.')<br />');
            }

            $wgOut->addHTML(
                    Xml::submitButton('Mark As Updated') .
                    Xml::closeElement('form') .
                    Xml::closeElement('fieldset')
            );
        }
    }

}