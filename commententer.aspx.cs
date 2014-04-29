﻿using System;
using System.Activities.Expressions;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Commententer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        classDropDownList.Enabled = false;
        
        if (classDropDownList.SelectedIndex == 0)
        {
            addCommentButton.Enabled = false;
        }

        if (!IsPostBack)
        {
            if (Session["stickyTerm"] != null)
            {
                termDropDownList.SelectedValue = Session["stickyTerm"].ToString();
                classDropDownList.Enabled = true;
            }
        }

        if (termDropDownList.SelectedIndex > 0)
        {
            classDropDownList.Enabled = true;
        }
    }

    protected void ClassDDLIndexChangedEvent(object sender, EventArgs e)
    {
        if (classDropDownList.SelectedIndex > 0)
        {
            addCommentButton.Enabled = true;
        }

        CommentTextBox.Focus();
    }

    protected void TermDDLIndexChangedEvent(object sender, EventArgs e)
    {
        StickyTermSelected();

        if (termDropDownList.SelectedIndex == 0)
        {
            classDropDownList.SelectedIndex = 0;
            addCommentButton.Enabled = false;
        }

        classDropDownList.Items.Clear();
        var dummyItem = new ListItem {Value = "-1", Text = "--select a class/section--"};
        classDropDownList.Items.Insert(0, dummyItem);
        if (termDropDownList.SelectedIndex > 0)
        {
            classDropDownList.DataBind();         
        }
    }

    protected void StickyTermSelected()
    {
        if (termDropDownList.SelectedIndex != 0)
        {
            Session["stickyTerm"] = termDropDownList.SelectedValue;
        }
    }

    protected void addCommentButton_Click(object sender, EventArgs e)
    {
        insertMessageLabel.Text = String.Empty;
        exceptionMessageLabel.Text = String.Empty;

        try
        {
            var objDS = new SqlDataSource();
            objDS.ProviderName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ProviderName;
            objDS.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            objDS.InsertCommand = "INSERT INTO COMMENT (ClassNum, Term, StudentComments) VALUES (@Class, @Term, @StudentComments)" ;
            objDS.InsertParameters.Add("Class", classDropDownList.SelectedValue);
            objDS.InsertParameters.Add("Term", termDropDownList.SelectedValue);
            objDS.InsertParameters.Add("StudentComments", CommentTextBox.Text.Trim());

            objDS.Insert();
            
            CommentTextBox.Text = String.Empty;
            CommentTextBox.Focus();
        }
        catch (Exception ex)
        {
            insertMessageLabel.Text =
                "There was an issue adding the comment to the database.  Please make sure you have selected a term and class in the drop-down menus.  If the problem persists, please restart the application and try again.";
            exceptionMessageLabel.Text = ex.Message;
        }

    }
}