
/*
    angular-treeview.js
*/
(function (l) {
    l.module("angularTreeview", []).directive("treeModel", function ($compile) {
        return {
            restrict: "A", link: function (a, g, c) {
                var e = c.treeModel, h = c.nodeLabel || "label", d = c.nodeChildren || "children", k = '<div class="content-common" data-ng-repeat="node in ' + e + '">'
                    + '<div class="page-name"> <span class="fa fa-plus-square collapsed" data-ng-show="node.' + d + '.length && node.collapsed" data-ng-click="selectNodeHead(node, $event)"></span>'
                    + '<span class="fa fa-minus-square expanded" data-ng-show="node.' + d + '.length && !node.collapsed" data-ng-click="selectNodeHead(node, $event)"></span>'
                    + '<span class="fa fa-square normal" data-ng-hide="node.' + d + '.length"></span> {{node.' + h + '}} </div> '
                    + '<div class="page-s-title" ng-repeat="p in [1,2,3]"> '
                    + '<input type="radio" value="{{p}}" ng-model="node.PrivilegeId" name="{{node.ObjectId}}" id="{{p}}{{node.ObjectId}}">'
                    + '<label for="{{p}}{{node.ObjectId}}" class="ng-binding"></label>'
                    + '</div>'

                    //ALLOW CONFIDENTIAL DOCUMENT TYPE(S)?
                    + '<div class="page-s-label" ng-if="node.ObjectId==92">'
                    + '<span>'
                    + '<input type="checkbox" ng-model="node.AllowConfidentialDocumentType" id="chk_{{node.ObjectId}}"> '
                    + '<label for="chk_{{node.ObjectId}}" class="ng-binding">ALLOW CONFIDENTIAL DOCUMENT TYPE(S)?</label>'
                    + '<span>'
                    + '</div>'

                    //Allow APQP Pricing Access?, Allow APQP/Change Request delete record?
                    + '<div class="page-s-label" ng-if="node.ObjectId==105">'
                    + '<span>'
                    + '<input type="checkbox" ng-model="node.HasPricingFieldsAccess" id="chkPA_{{node.ObjectId}}"> '
                    + '<label for="chkPA_{{node.ObjectId}}" class="ng-binding">Allow APQP Pricing Access?</label>'
                    + '</span>'
                    + '<span>'
                    + '<input type="checkbox" ng-model="node.AllowDeleteRecord" id="chkDR_{{node.ObjectId}}"> '
                    + '<label for="chkDR_{{node.ObjectId}}" class="ng-binding">Allow APQP/Change Request delete record?</label>'
                    + '</span>'
                    + '</div>'

                    //Allow Send Data To SAP?, Allow Update NPIF Status?
                    + '<div class="page-s-label" ng-if="node.ObjectId==107">'
                    + '<span>'
                    + '<input type="checkbox" ng-model="node.AllowSendDataToSAP" id="chkKickOffSendData_{{node.ObjectId}}"> '
                    + '<label for="chkKickOffSendData_{{node.ObjectId}}" class="ng-binding">Allow Send Data To SAP?</label>'
                    + '</span>'
                    + '<span>'
                    + '<input type="checkbox" ng-model="node.AllowCheckNPIFStatus" id="chkNPIFStatus_{{node.ObjectId}}"> '
                    + '<label for="chkNPIFStatus_{{node.ObjectId}}" class="ng-binding">Allow to check NPIF status?</label>'
                    + '</span>'
                    + '</div>'

                    //Allow Export To SAP?
                    + '<div class="page-s-label" ng-if="node.ObjectId==111">'
                    + '<span>'
                    + '<input type="checkbox" ng-model="node.AllowExportToSAP" id="chkPPAP_{{node.ObjectId}}"> '
                    + '<label for="chkPPAP_{{node.ObjectId}}" class="ng-binding">Allow Export To SAP?</label>'
                    + '<span>'
                    + '</div>'

                    //Allow APQP Pricing Access? for change request
                    + '<div class="page-s-label" ng-if="node.ObjectId==106">'
                    + '<span>'
                    + '<input type="checkbox" ng-model="node.HasPricingFieldsAccess" id="chkCR_{{node.ObjectId}}"> '
                    + '<label for="chkCR_{{node.ObjectId}}" class="ng-binding">Allow APQP Pricing Access?</label>'
                    + '<span>'
                    + '</div>'

                    + '<div data-ng-hide="node.collapsed" data-tree-model="node.' + d + '" data-node-id=' + (c.nodeId || "id") + " data-node-label=" + h + " data-node-children=" + d + "></div> </div>";
                e && e.length && (c.angularTreeview ? (a.$watch(e, function (m, b) { g.empty().html($compile(k)(a)) }, !1), a.selectNodeHead = a.selectNodeHead || function (a, b) {
                    b.stopPropagation && b.stopPropagation(); b.preventDefault && b.preventDefault(); b.cancelBubble =
                    !0; b.returnValue = !1; a.collapsed = !a.collapsed
                }, a.selectNodeLabel = a.selectNodeLabel || function (c, b) {
                    b.stopPropagation && b.stopPropagation(); b.preventDefault && b.preventDefault(); b.cancelBubble = !0; b.returnValue = !1;
                    a.currentNode && a.currentNode.selected && (a.currentNode.selected = void 0); c.selected = "selected"; a.currentNode = c
                }) : g.html($compile(k)(a)))
            }
        }
    })
})(angular);