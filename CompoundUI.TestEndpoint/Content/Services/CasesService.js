//@*********************************************************************************************************************@
//@**                             This Service is owned by the Cases Capability                                       **@
//@**                 Any changes should be made in Cases.Views or they will be overwritten                           **@
//@*********************************************************************************************************************@

'use strict';

CMAPP.factory('CasesService', function ($http, URLConfig) {
    var allCases = {};
    var updateLocalCases = function (data) {
        var caseToUpdate = $.grep(allCases, function (_case) { return _case.Id === data.Id })[0];
        angular.copy(data, caseToUpdate);
    };

    return {
        url: URLConfig.url('Cases'),
        create: function (data, success, error) {
            return $http
                .post(
                    this.url + '/Create',
                    data
                )
                .success(function (data, status) {
                    success(data, status);
                })
                .error(function (data, status) {
                    error(data, status);
                });
        },
        getAllCases: function () {
            return $http
                .get(
                    this.url + '/GetUserCases'
                ).then(function (response) {
                    allCases = response.data;
                    return allCases;
                });
        },
        getCaseById: function (caseID) {
            return $http
                .get(
                    this.url + '/loadcase/' + caseID
                );
        },
        connectUsersToCase: function (data) {
            return $http
                .post(
                    this.url + '/ConnectUsers',
                    data
                );
        },
        addCaseNote: function (data) {
            return $http
                .post(
                    this.url + '/AddCaseNotes',
                    data
                );
        },
        updateCaseDetails: function (data, success, error) {
            var updatedData = data;
            return $http.put(this.url + '/updateCaseDetails', data)
                        .success(function (data, status) {
                            updateLocalCases(updatedData);
                            success(data, status);
                        })
                        .error(function (data, status) {
                            error(data, status);
                        });
        }
    };
});