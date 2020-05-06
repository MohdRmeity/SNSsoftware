<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Cufex_MasterHeader.ascx.vb" Inherits="SNSsoftware.Cufex_MasterHeader" %>
<div>
    <div class="DivHeader"></div>
    <div style="height:12px;"></div>
    <div class="u-ht-xs-3"></div>
    <div class="NormalDiv1100Max">
        
        <div class="floatR">
            <div class="floatL u-ht-xs-45">
                <div class="u-ht-xs-5"></div>
                <div class="u-ht-xs-3"></div>
                <div class="WelcomeDiv AnimateMe BlueMouseOver  u-mr-xs-20 openProfMenu">
                    <asp:Label ID="lblWelcome" runat="server"></asp:Label>
                </div>
            </div>
            <div class="floatL  u-ht-xs-45  u-mr-xs-10 openProfMenu u-pointer">
                <img id="imgProfile" runat="server" src='#' alt="" class="ProfileImage u-circle" />
            </div>
            <div class="floatL u-relative">
                <div class="u-ht-xs-15"></div>
                <div class="openProfMenu iconOpen u-pointer u-animateMe">
                    <span class="icon ion-ios-arrow-down ArrowProf"></span>
                </div>
                <div class="ProfileData u-hide">
                    <div class="u-pb-xs-15">
                        <div class="u-relative HoverProf">
                            <a href="<%= Page.GetRouteUrl("SNSsoftware-Cufex-Security_Users", Nothing) & "?user=" & Session("BUserCode")%>" class="u-imgLink"></a>
                            <div class="u-inlineBlock">
                                <span class="icon ion-ios-contact PMenuIcon"></span>
                            </div>
                            <div class="u-inlineBlock valignT">
                                <div class="u-ht-xs-5"></div>
                                <span class="textalignL PMenuItemName u-animateMe">Edit Profile</span>
                            </div>
                        </div>
                    </div>

                  <%--  <div class="u-pb-xs-15">
                        <div class="u-relative HoverProf">
                            <a href="#" class="u-imgLink"></a>
                            <div class="u-inlineBlock">
                                <span class="icon ion-md-help-circle PMenuIcon"></span>
                            </div>
                            <div class="u-inlineBlock valignT">
                                <div class="u-ht-xs-5"></div>
                                <span class="textalignL PMenuItemName u-animateMe">Get Help</span>
                            </div>
                        </div>
                    </div>--%>

                    <div class="u-pb-xs-15">
                        <div class="u-relative HoverProf">
                            <a href="<%= sAppPath %>Controls/Cufex_PlugIns/Cufex_Logout.aspx" class="u-imgLink"></a>
                            <div class="u-inlineBlock">
                                <span class="icon ion-ios-log-in PMenuIcon"></span>
                            </div>
                            <div class="u-inlineBlock valignT">
                                <div class="u-ht-xs-5"></div>
                                <span class="textalignL PMenuItemName u-animateMe">Logout</span>
                            </div>
                        </div>
                    </div>

                    <%--  <div class="logOutDiv AnimateMe BlueMouseOver GoToChildOnClick">
                             <a href="<%= sAppPath %>Controls/Cufex_PlugIns/Cufex_Logout.aspx">
                             </a>
                         </div>--%>
                </div>
            </div>
        </div>
        <%--<div class="floatR">
            <div style="height:5px;"></div>
            <div class="u-inlineBlock">
                <a href="<%= Page.GetRouteUrl("SNSsoftware-Cufex-Security_Settings_General", Nothing) %>"><span class="icon ion-ios-settings MenuIcon" style="font-size:34px;"></span></a>
            </div>
        </div>--%>
    </div>
</div>
