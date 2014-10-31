//@*********************************************************************************************************************@
//@**                         This Controller is owned by the Cases Capability                                        **@
//@**                Any changes should be made in Cases.Views or they will be overwritten                            **@
//@*********************************************************************************************************************@

/**
  * Controller for listing cases
*/

'use strict';

CMAPP.controller('ListCasesController', function ($scope, allCases) {

    $scope.cases = {
        allCases: allCases
    };

});