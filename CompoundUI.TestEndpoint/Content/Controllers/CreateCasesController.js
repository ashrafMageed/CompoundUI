//@*********************************************************************************************************************@
//@**                         This Controller is owned by the Cases Capability                                        **@
//@**                Any changes should be made in Cases.Views or they will be overwritten                            **@
//@*********************************************************************************************************************@

/**
  * Controller for the create case form.
*/

'use strict';

CMAPP.controller('CreateCasesController', function ($scope, CasesService, common, $state) {

    var self = this;

    // Will hold the case details
    $scope.cases = {};

    // Scope object for display purposes only
    $scope.screen = {
        caseName: ($scope.cases.name && $scope.cases.name.length ? ': ' + $scope.cases.name : '')
    };

    // Change the view as the user adds a name
    $scope.$watch('cases.name', function (newValue, oldValue) {
        $scope.screen.caseName = newValue ? ': ' + newValue : '';
        self.changeBreadcrumb($scope.screen.caseName);
    });

    // Called when the submit button is clicked. Passes off to our CasesService
    $scope.submitCaseForm = function () {
        $scope.cases.caseID = common.getNewUUID();
        CasesService.create($scope.cases, self.caseFormSuccess, self.caseFormError);
    };

    // Called when the reset button is clicked. 
    $scope.caseFormReset = function () {
        $scope.cases = {}; // Reset the case object
        $scope.caseForm.$setPristine(); // Make the form pristine again
    };

    // Updates the breadcrumb as the user changes the name
    this.changeBreadcrumb = function(value) {
        $state.current.data.ncyBreadcrumbLabel = "Create Case" + value;
        $scope.$new().$emit('$viewContentLoaded');
    };

    // Success callback for the CaseFormService
    this.caseFormSuccess = function(data) {
        $scope.addedCase = {}; // Create a temp object for our success message
        // Create a messages object, and set it's success property to true
        $scope.messages = {};
        $scope.messages.success = true;
        // Copy the user object into the addedUser object for the view
        angular.copy($scope.cases, $scope.addedCase);
        // Reset everything
        $scope.caseFormReset();
    };

    // Error callback for the CaseFormService
    this.caseFormError = function(data) {
        // Create a messages object, and set it's errors property to true
        $scope.messages = {};
        $scope.messages.errors = true;
        // TODO : This should be passed in from the server, then we'll need to loop over the errorFields (or whatever it will be)
        //$scope.messages.errorFields = {};
        //$scope.cases.description = '';
        //$scope.messages.errorFields.description = 'Description field cannot be blank.';
    };

});
