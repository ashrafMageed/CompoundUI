//@*********************************************************************************************************************@
//@**                         This Controller is owned by the Cases Capability                                        **@
//@**                Any changes should be made in Cases.Views or they will be overwritten                            **@
//@*********************************************************************************************************************@

/**
  * Controller for viewing a case cases
*/

'use strict';

CMAPP.controller('CaseDetailsController', function ($scope, $state, caseById, CasesService) {
    $scope.mode = "view";
    $scope.isDisabled = true;
    $scope.cases = {
        caseDetails: caseById
    };

    var connectedUsers = caseById.ConnectedUsersIds ? caseById.ConnectedUsersIds : [],
        connectedUserIDs = [],
        connectedUserIDsString;

    if (connectedUsers.length) {
        for (var i = 0; i < connectedUsers.length; i += 1) {
            connectedUserIDs.push(connectedUsers[i]);
        }
    }

    var setToViewMode = function (edit) {
        edit = edit || false;
        $scope.mode = edit ? "edit" : "view";
        $scope.isDisabled = !edit;
        $scope.messages = {};
    };

    var success = function () {
        $scope.messages = {};
        $scope.messages.success = true;
    };

    var error = function () {
        $scope.messages = {};
        $scope.messages.errors = true;
    };

    $scope.viewConnectedUsers = function () {
        $state.go("manage.cases.case.viewusers", { userIDsString: connectedUserIDs });
    };

    $scope.edit = function () {
        var edit = !(!!($scope.mode === 'edit'));
        setToViewMode(edit);
        $scope.master = angular.copy($scope.cases.caseDetails);
    };

    $scope.cancel = function () {
        setToViewMode();
        $scope.cases.caseDetails = angular.copy($scope.master);
        $scope.messages = {};
    };

    var isUnchanged = function () {
        return angular.equals($scope.cases.caseDetails, $scope.master);
    };

    $scope.canSave = function () {
        return $scope.caseForm.$invalid || isUnchanged();
    };

    $scope.submitCaseDescription = function () {
        setToViewMode();
        CasesService.updateCaseDetails($scope.cases.caseDetails, success, error);
    };
});