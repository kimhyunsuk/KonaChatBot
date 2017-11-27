﻿using KonaChatBot.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace KonaChatBot.DB
{
    public class DbConnect
    {
        //string connStr = "Data Source=taihoinst.database.windows.net;Initial Catalog=taihoLab;User ID=taihoinst;Password=taiho123@;";
        string connStr = "Data Source=taihoinst.database.windows.net;Initial Catalog=taihoLab;User ID=taihoinst;Password=taiho123@;";
        StringBuilder sb = new StringBuilder();
        public readonly string TEXTDLG = "2";
        public readonly string CARDDLG = "3";
        public readonly string MEDIADLG = "4";

        public void ConnectDb()
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(connStr);
                conn.Open();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }


        public List<DialogList> SelectInitDialog(String channel)
        {
            SqlDataReader rdr = null;
            List<DialogList> dialogs = new List<DialogList>();
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText += " SELECT   				    ";
                cmd.CommandText += " 	DLG_ID,                 ";
                cmd.CommandText += " 	DLG_TYPE,               ";
                cmd.CommandText += " 	DLG_GROUP,              ";
                cmd.CommandText += " 	DLG_ORDER_NO            ";
                cmd.CommandText += " FROM TBL_DLG               ";
                cmd.CommandText += " WHERE DLG_GROUP = '1'      ";
                cmd.CommandText += " AND USE_YN = 'Y'           ";
                cmd.CommandText += " ORDER BY DLG_ID            ";

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                
                                
                while (rdr.Read())
                {
                    DialogList dlg = new DialogList();
                    dlg.dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    dlg.dlgType = rdr["DLG_TYPE"] as string;
                    dlg.dlgGroup = rdr["DLG_GROUP"] as string;
                    dlg.dlgOrderNo = rdr["DLG_ORDER_NO"] as string;
                    
                    using (SqlConnection conn2 = new SqlConnection(connStr))
                    {
                        SqlCommand cmd2 = new SqlCommand();
                        conn2.Open();
                        cmd2.Connection = conn2;
                        SqlDataReader rdr2 = null;
                        if (dlg.dlgType.Equals(TEXTDLG))
                        {
                            cmd2.CommandText = "SELECT CARD_TITLE, CARD_TEXT FROM TBL_DLG_TEXT WHERE DLG_ID = @dlgID AND USE_YN = 'Y'";
                            cmd2.Parameters.AddWithValue("@dlgID", dlg.dlgId);
                            rdr2 = cmd2.ExecuteReader(CommandBehavior.CloseConnection);

                            while (rdr2.Read())
                            {
                                dlg.cardTitle = rdr2["CARD_TITLE"] as string;
                                dlg.cardText = rdr2["CARD_TEXT"] as string;
                            }
                            rdr2.Close();
                        } else if (dlg.dlgType.Equals(CARDDLG))
                        {
                            cmd2.CommandText = "SELECT CARD_TITLE, CARD_SUBTITLE, CARD_TEXT, IMG_URL," +
                                    "BTN_1_TYPE, BTN_1_TITLE, BTN_1_CONTEXT, BTN_2_TYPE, BTN_2_TITLE, BTN_2_CONTEXT, BTN_3_TYPE, BTN_3_TITLE, BTN_3_CONTEXT, BTN_4_TYPE, BTN_4_TITLE, BTN_4_CONTEXT, " +
                                    "CARD_DIVISION, CARD_VALUE " +
                                    "FROM TBL_DLG_CARD WHERE DLG_ID = @dlgID AND USE_YN = 'Y' ";
                            if (channel.Equals("facebook"))
                            {
                                cmd2.CommandText += "FB_USE_YN = 'Y' ";
                            }
                            cmd2.CommandText += "ORDER BY CARD_ORDER_NO";
                            cmd2.Parameters.AddWithValue("@dlgID", dlg.dlgId);
                            rdr2 = cmd2.ExecuteReader(CommandBehavior.CloseConnection);
                            List<CardList> dialogCards = new List<CardList>();
                            while (rdr2.Read())
                            {
                                CardList dlgCard = new CardList();
                                dlgCard.cardTitle = rdr2["CARD_TITLE"] as string;
                                dlgCard.cardSubTitle = rdr2["CARD_SUBTITLE"] as string;
                                dlgCard.cardText = rdr2["CARD_TEXT"] as string;
                                dlgCard.imgUrl = rdr2["IMG_URL"] as string;
                                dlgCard.btn1Type = rdr2["BTN_1_TYPE"] as string;
                                dlgCard.btn1Title = rdr2["BTN_1_TITLE"] as string;
                                dlgCard.btn1Context = rdr2["BTN_1_CONTEXT"] as string;
                                dlgCard.btn2Type = rdr2["BTN_2_TYPE"] as string;
                                dlgCard.btn2Title = rdr2["BTN_2_TITLE"] as string;
                                dlgCard.btn2Context = rdr2["BTN_2_CONTEXT"] as string;
                                dlgCard.btn3Type = rdr2["BTN_3_TYPE"] as string;
                                dlgCard.btn3Title = rdr2["BTN_3_TITLE"] as string;
                                dlgCard.btn3Context = rdr2["BTN_3_CONTEXT"] as string;
                                dlgCard.btn4Type = rdr2["BTN_4_TYPE"] as string;
                                dlgCard.btn4Title = rdr2["BTN_4_TITLE"] as string;
                                dlgCard.btn4Context = rdr2["BTN_4_CONTEXT"] as string;
                                dlgCard.cardDivision = rdr2["CARD_DIVISION"] as string;
                                dlgCard.cardValue = rdr2["CARD_VALUE"] as string;
                                dialogCards.Add(dlgCard);
                            }
                            dlg.dialogCard = dialogCards;
                        } else if (dlg.dlgType.Equals(MEDIADLG))
                        {
                            cmd2.CommandText = "SELECT CARD_TITLE, CARD_TEXT, MEDIA_URL," +
                                    "BTN_1_TYPE, BTN_1_TITLE, BTN_1_CONTEXT, BTN_2_TYPE, BTN_2_TITLE, BTN_2_CONTEXT, BTN_3_TYPE, BTN_3_TITLE, BTN_3_CONTEXT, BTN_4_TYPE, BTN_4_TITLE, BTN_4_CONTEXT " +
                                    "FROM TBL_DLG_MEDIA WHERE DLG_ID = @dlgID AND USE_YN = 'Y'";
                            cmd2.Parameters.AddWithValue("@dlgID", dlg.dlgId);
                            rdr2 = cmd2.ExecuteReader(CommandBehavior.CloseConnection);

                            while (rdr2.Read())
                            {
                                dlg.cardTitle = rdr2["CARD_TITLE"] as string;
                                dlg.cardText = rdr2["CARD_TEXT"] as string;
                                dlg.mediaUrl = rdr2["MEDIA_URL"] as string;
                                dlg.btn1Type = rdr2["BTN_1_TYPE"] as string;
                                dlg.btn1Title = rdr2["BTN_1_TITLE"] as string;
                                dlg.btn1Context = rdr2["BTN_1_CONTEXT"] as string;
                                dlg.btn2Type = rdr2["BTN_2_TYPE"] as string;
                                dlg.btn2Title = rdr2["BTN_2_TITLE"] as string;
                                dlg.btn2Context = rdr2["BTN_2_CONTEXT"] as string;
                                dlg.btn3Type = rdr2["BTN_3_TYPE"] as string;
                                dlg.btn3Title = rdr2["BTN_3_TITLE"] as string;
                                dlg.btn3Context = rdr2["BTN_3_CONTEXT"] as string;
                                dlg.btn4Type = rdr2["BTN_4_TYPE"] as string;
                                dlg.btn4Title = rdr2["BTN_4_TITLE"] as string;
                                dlg.btn4Context = rdr2["BTN_4_CONTEXT"] as string;
                            }
                        }
                        
                    }
                    dialogs.Add(dlg);
                }
                rdr.Close();
            }
            return dialogs;
        }

        public DialogList SelectDialog(int dlgID)
        {
            SqlDataReader rdr = null;
            DialogList dlg = new DialogList();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                cmd.CommandText += " SELECT   				    ";
                cmd.CommandText += " 	DLG_ID,                 ";
                cmd.CommandText += " 	DLG_NAME,               ";
                cmd.CommandText += " 	DLG_DESCRIPTION,        ";
                cmd.CommandText += " 	DLG_LANG,               ";
                cmd.CommandText += " 	DLG_TYPE,               ";
                cmd.CommandText += " 	DLG_ORDER_NO,           ";
                cmd.CommandText += " 	DLG_GROUP               ";
                cmd.CommandText += " FROM TBL_DLG               ";
                cmd.CommandText += " WHERE DLG_ID = @dlgId      ";
                cmd.CommandText += " AND USE_YN = 'Y'           ";

                cmd.Parameters.AddWithValue("@dlgID", dlgID);
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    dlg.dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    dlg.dlgType = rdr["DLG_TYPE"] as string;
                    dlg.dlgGroup = rdr["DLG_GROUP"] as string;
                    dlg.dlgOrderNo = rdr["DLG_ORDER_NO"] as string;

                    using (SqlConnection conn2 = new SqlConnection(connStr))
                    {
                        SqlCommand cmd2 = new SqlCommand();
                        conn2.Open();
                        cmd2.Connection = conn2;
                        SqlDataReader rdr2 = null;
                        if (dlg.dlgType.Equals(TEXTDLG))
                        {
                            cmd2.CommandText = "SELECT CARD_TITLE, CARD_TEXT FROM TBL_DLG_TEXT WHERE DLG_ID = @dlgID AND USE_YN = 'Y'";
                            cmd2.Parameters.AddWithValue("@dlgID", dlg.dlgId);
                            rdr2 = cmd2.ExecuteReader(CommandBehavior.CloseConnection);

                            while (rdr2.Read())
                            {
                                dlg.cardTitle = rdr2["CARD_TITLE"] as string;
                                dlg.cardText = rdr2["CARD_TEXT"] as string;
                            }
                            rdr2.Close();
                        }
                        else if (dlg.dlgType.Equals(CARDDLG))
                        {
                            cmd2.CommandText = "SELECT CARD_TITLE, CARD_SUBTITLE, CARD_TEXT, IMG_URL," +
                                    "BTN_1_TYPE, BTN_1_TITLE, BTN_1_CONTEXT, BTN_2_TYPE, BTN_2_TITLE, BTN_2_CONTEXT, BTN_3_TYPE, BTN_3_TITLE, BTN_3_CONTEXT, BTN_4_TYPE, BTN_4_TITLE, BTN_4_CONTEXT, " +
                                    "CARD_DIVISION, CARD_VALUE, CARD_ORDER_NO " +
                                    "FROM TBL_DLG_CARD WHERE DLG_ID = @dlgID AND USE_YN = 'Y' ORDER BY CARD_ORDER_NO";
                            cmd2.Parameters.AddWithValue("@dlgID", dlg.dlgId);
                            rdr2 = cmd2.ExecuteReader(CommandBehavior.CloseConnection);
                            List<CardList> dialogCards = new List<CardList>();
                            while (rdr2.Read())
                            {
                                CardList dlgCard = new CardList();
                                dlgCard.cardTitle = rdr2["CARD_TITLE"] as string;
                                dlgCard.cardSubTitle = rdr2["CARD_SUBTITLE"] as string;
                                dlgCard.cardText = rdr2["CARD_TEXT"] as string;
                                dlgCard.imgUrl = rdr2["IMG_URL"] as string;
                                dlgCard.btn1Type = rdr2["BTN_1_TYPE"] as string;
                                dlgCard.btn1Title = rdr2["BTN_1_TITLE"] as string;
                                dlgCard.btn1Context = rdr2["BTN_1_CONTEXT"] as string;
                                dlgCard.btn2Type = rdr2["BTN_2_TYPE"] as string;
                                dlgCard.btn2Title = rdr2["BTN_2_TITLE"] as string;
                                dlgCard.btn2Context = rdr2["BTN_2_CONTEXT"] as string;
                                dlgCard.btn3Type = rdr2["BTN_3_TYPE"] as string;
                                dlgCard.btn3Title = rdr2["BTN_3_TITLE"] as string;
                                dlgCard.btn3Context = rdr2["BTN_3_CONTEXT"] as string;
                                dlgCard.btn4Type = rdr2["BTN_4_TYPE"] as string;
                                dlgCard.btn4Title = rdr2["BTN_4_TITLE"] as string;
                                dlgCard.btn4Context = rdr2["BTN_4_CONTEXT"] as string;
                                dlgCard.cardDivision = rdr2["CARD_DIVISION"] as string;
                                dlgCard.cardValue = rdr2["CARD_VALUE"] as string;
                                dlgCard.card_order_no = rdr2["CARD_ORDER_NO"] as string;
                                
                                dialogCards.Add(dlgCard);
                            }
                            dlg.dialogCard = dialogCards;
                        }
                        else if (dlg.dlgType.Equals(MEDIADLG))
                        {
                            cmd2.CommandText = "SELECT CARD_TITLE, CARD_TEXT, MEDIA_URL," +
                                    "BTN_1_TYPE, BTN_1_TITLE, BTN_1_CONTEXT, BTN_2_TYPE, BTN_2_TITLE, BTN_2_CONTEXT, BTN_3_TYPE, BTN_3_TITLE, BTN_3_CONTEXT, BTN_4_TYPE, BTN_4_TITLE, BTN_4_CONTEXT " +
                                    "FROM TBL_DLG_MEDIA WHERE DLG_ID = @dlgID AND USE_YN = 'Y'";
                            cmd2.Parameters.AddWithValue("@dlgID", dlg.dlgId);
                            rdr2 = cmd2.ExecuteReader(CommandBehavior.CloseConnection);

                            while (rdr2.Read())
                            {
                                dlg.cardTitle = rdr2["CARD_TITLE"] as string;
                                dlg.cardText = rdr2["CARD_TEXT"] as string;
                                dlg.mediaUrl = rdr2["MEDIA_URL"] as string;
                                dlg.btn1Type = rdr2["BTN_1_TYPE"] as string;
                                dlg.btn1Title = rdr2["BTN_1_TITLE"] as string;
                                dlg.btn1Context = rdr2["BTN_1_CONTEXT"] as string;
                                dlg.btn2Type = rdr2["BTN_2_TYPE"] as string;
                                dlg.btn2Title = rdr2["BTN_2_TITLE"] as string;
                                dlg.btn2Context = rdr2["BTN_2_CONTEXT"] as string;
                                dlg.btn3Type = rdr2["BTN_3_TYPE"] as string;
                                dlg.btn3Title = rdr2["BTN_3_TITLE"] as string;
                                dlg.btn3Context = rdr2["BTN_3_CONTEXT"] as string;
                                dlg.btn4Type = rdr2["BTN_4_TYPE"] as string;
                                dlg.btn4Title = rdr2["BTN_4_TITLE"] as string;
                                dlg.btn4Context = rdr2["BTN_4_CONTEXT"] as string;
                            }
                        }

                    }
                }
            }
            return dlg;
        }

        public List<CardList> SelectDialogCard(int dlgID)
        {
            SqlDataReader rdr = null;
            List<CardList> dialogCard = new List<CardList>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT CARD_DLG_ID, DLG_ID, CARD_TITLE, CARD_SUBTITLE, CARD_TEXT, IMG_URL," +
                    "BTN_1_TYPE, BTN_1_TITLE, BTN_1_CONTEXT, BTN_2_TYPE, BTN_2_TITLE, BTN_2_CONTEXT, BTN_3_TYPE, BTN_3_TITLE, BTN_3_CONTEXT, " +
                    "CARD_DIVISION, CARD_VALUE " +
                    "FROM TBL_DLG_CARD WHERE DLG_ID = @dlgID AND USE_YN = 'Y' AND DLG_ID > 999 ORDER BY CARD_ORDER_NO";
                    //"FROM TBL_SECCS_DLG_CARD WHERE DLG_ID = @dlgID AND USE_YN = 'Y' AND DLG_ID > 999 ORDER BY CARD_ORDER_NO";

                cmd.Parameters.AddWithValue("@dlgID", dlgID);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int cardDlgId = Convert.ToInt32(rdr["CARD_DLG_ID"]);
                    int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    string cardTitle = rdr["CARD_TITLE"] as string;
                    string cardSubTitle = rdr["CARD_SUBTITLE"] as string;
                    string cardText = rdr["CARD_TEXT"] as string;
                    string imgUrl = rdr["IMG_URL"] as string;
                    string btn1Type = rdr["BTN_1_TYPE"] as string;
                    string btn1Title = rdr["BTN_1_TITLE"] as string;
                    string btn1Context = rdr["BTN_1_CONTEXT"] as string;
                    string btn2Type = rdr["BTN_2_TYPE"] as string;
                    string btn2Title = rdr["BTN_2_TITLE"] as string;
                    string btn2Context = rdr["BTN_2_CONTEXT"] as string;
                    string btn3Type = rdr["BTN_3_TYPE"] as string;
                    string btn3Title = rdr["BTN_3_TITLE"] as string;
                    string btn3Context = rdr["BTN_3_CONTEXT"] as string;
                    string cardDivision = rdr["CARD_DIVISION"] as string;
                    string cardValue = rdr["CARD_VALUE"] as string;

                    CardList dlgCard = new CardList();
                    dlgCard.cardDlgId = cardDlgId;
                    dlgCard.dlgId = dlgId;
                    dlgCard.cardTitle = cardTitle;
                    dlgCard.cardSubTitle = cardSubTitle;
                    dlgCard.cardText = cardText;
                    dlgCard.imgUrl = imgUrl;
                    dlgCard.btn1Type = btn1Type;
                    dlgCard.btn1Title = btn1Title;
                    dlgCard.btn1Context = btn1Context;
                    dlgCard.btn2Type = btn2Type;
                    dlgCard.btn2Title = btn2Title;
                    dlgCard.btn2Context = btn2Context;
                    dlgCard.btn3Type = btn3Type;
                    dlgCard.btn3Title = btn3Title;
                    dlgCard.btn3Context = btn3Context;
                    dlgCard.cardDivision = cardDivision;
                    dlgCard.cardValue = cardValue;

                    dialogCard.Add(dlgCard);
                }
            }
            return dialogCard;
        }

        public List<TextList> SelectDialogText(int dlgID)
        {
            SqlDataReader rdr = null;
            List<TextList> dialogText = new List<TextList>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT TEXT_DLG_ID, DLG_ID, CARD_TITLE, CARD_TEXT FROM TBL_DLG_TEXT WHERE DLG_ID = @dlgID AND USE_YN = 'Y' AND DLG_ID > 999";
                //cmd.CommandText = "SELECT TEXT_DLG_ID, DLG_ID, CARD_TITLE, CARD_TEXT FROM TBL_SECCS_DLG_TEXT WHERE DLG_ID = @dlgID AND USE_YN = 'Y' AND DLG_ID > 999";

                cmd.Parameters.AddWithValue("@dlgID", dlgID);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int textDlgId = Convert.ToInt32(rdr["TEXT_DLG_ID"]);
                    int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    string cardTitle = rdr["CARD_TITLE"] as string;
                    string cardText = rdr["CARD_TEXT"] as string;


                    TextList dlgText = new TextList();
                    dlgText.textDlgId = textDlgId;
                    dlgText.dlgId = dlgId;
                    dlgText.cardTitle = cardTitle;
                    dlgText.cardText = cardText;


                    dialogText.Add(dlgText);
                }
            }
            return dialogText;
        }


        public List<TextList> SelectSorryDialogText(string dlgGroup)
        {
            SqlDataReader rdr = null;
            List<TextList> dialogText = new List<TextList>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT TEXT_DLG_ID, DLG_ID, CARD_TITLE,CARD_TEXT FROM TBL_DLG_TEXT WHERE DLG_ID = (SELECT DLG_ID FROM TBL_DLG WHERE DLG_GROUP = @dlgGroup) AND USE_YN = 'Y'";
                //cmd.CommandText = "SELECT TEXT_DLG_ID, DLG_ID, CARD_TITLE, CARD_TEXT FROM TBL_SECCS_DLG_TEXT WHERE DLG_ID = @dlgID AND USE_YN = 'Y' AND DLG_ID > 999";

                cmd.Parameters.AddWithValue("@dlgGroup", dlgGroup);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int textDlgId = Convert.ToInt32(rdr["TEXT_DLG_ID"]);
                    int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    string cardTitle = rdr["CARD_TITLE"] as string;
                    string cardText = rdr["CARD_TEXT"] as string;


                    TextList dlgText = new TextList();
                    dlgText.textDlgId = textDlgId;
                    dlgText.dlgId = dlgId;
                    dlgText.cardTitle = cardTitle;
                    dlgText.cardText = cardText;


                    dialogText.Add(dlgText);
                }
            }
            return dialogText;
        }
        
        
        //KSO START
        public CardList BannedChk(string orgMent)
        {
            SqlDataReader rdr = null;
            CardList SelectBanned = new CardList();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText += " SELECT                                                                                                                                                         ";
                cmd.CommandText += " TOP 1 TD.DLG_ID, (SELECT TOP 1 BANNED_WORD FROM TBL_BANNED_WORD_LIST WHERE CHARINDEX(BANNED_WORD, @msg) > 0) AS BANNED_WORD, TDT.CARD_TITLE, TDT.CARD_TEXT     ";
                cmd.CommandText += " FROM TBL_DLG TD, TBL_DLG_TEXT TDT                                                                                                                              ";
                cmd.CommandText += " WHERE TD.DLG_ID = TDT.DLG_ID                                                                                                                                   ";
                cmd.CommandText += " AND                                                                                                                                                            ";
                cmd.CommandText += " 	TD.DLG_GROUP =                                                                                                                                              ";
                cmd.CommandText += " 	(                                                                                                                                                           ";
                cmd.CommandText += " 	   SELECT CASE WHEN SUM(CASE WHEN BANNED_WORD_TYPE = 3 THEN CHARINDEX(A.BANNED_WORD, @msg) END) > 0 THEN 3                                                  ";
                cmd.CommandText += " 			  WHEN SUM(CASE WHEN BANNED_WORD_TYPE = 4 THEN CHARINDEX(A.BANNED_WORD, @msg) END) > 0 THEN 4                                                       ";
                cmd.CommandText += " 			 END                                                                                                                                                ";
                cmd.CommandText += " 	   FROM TBL_BANNED_WORD_LIST A                                                                                                                              ";
                cmd.CommandText += " 	) AND TD.DLG_GROUP IN (3,4)                                                                                                                                 ";
                cmd.CommandText += " ORDER BY NEWID()                                                                                                                                               ";

                cmd.Parameters.AddWithValue("@msg", orgMent);
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (rdr.Read())
                {
                    //answerMsg = rdr["CARD_TEXT"] + "@@" + rdr["DLG_ID"] + "@@" + rdr["CARD_TITLE"];

                    int dlg_id = Convert.ToInt32(rdr["DLG_ID"]);
                    String card_title = rdr["CARD_TITLE"] as String;
                    String card_text = rdr["CARD_TEXT"] as String;
                    
                    SelectBanned.dlgId = dlg_id;
                    SelectBanned.cardTitle = card_title;
                    SelectBanned.cardText = card_text;
                }
            }
            return SelectBanned;
        }

        public CacheList CacheChk(string orgMent)
        {
            SqlDataReader rdr = null;
            CacheList result = new CacheList();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText += "SELECT LUIS_ID, INTENT_ID, ENTITIES_IDS FROM TBL_QUERY_ANALYSIS_RESULT WHERE QUERY = @msg";

                cmd.Parameters.AddWithValue("@msg", orgMent);
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    string luisId = rdr["LUIS_ID"] as String;
                    string intentId = rdr["INTENT_ID"] as String;
                    string entitiesId = rdr["ENTITIES_IDS"] as String;

                    result.luisId = luisId;
                    result.luisIntent = intentId;
                    result.luisEntities = entitiesId;
                }
            }
            return result;
        }

        public List<RelationList> DefineTypeChk(string luisId, string intentId, string entitiesId)
        {
            SqlDataReader rdr = null;
            List<RelationList> result = new List<RelationList>();
            Debug.WriteLine("luisId ::: "+ luisId);
            Debug.WriteLine("intentId ::: " + intentId);
            Debug.WriteLine("entitiesId ::: " + entitiesId);
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText += "SELECT LUIS_ID, LUIS_INTENT, LUIS_ENTITIES, DLG_ID, DLG_API_DEFINE, API_ID ";
                cmd.CommandText += "  FROM TBL_DLG_RELATION_LUIS                                                    ";
                cmd.CommandText += " WHERE LUIS_INTENT = @intentId                                                 ";
                cmd.CommandText += "   AND LUIS_ENTITIES = @entities                                                ";
                cmd.CommandText += "   AND LUIS_ID = @luisId                                                        ";

                if(intentId != null){
                    cmd.Parameters.AddWithValue("@intentId", intentId);
                }else{
                    cmd.Parameters.AddWithValue("@intentId", DBNull.Value);
                }

                if (entitiesId != null){
                    cmd.Parameters.AddWithValue("@entities", entitiesId);
                }else{
                    cmd.Parameters.AddWithValue("@entities", DBNull.Value);
                }

                if (luisId != null){
                    cmd.Parameters.AddWithValue("@luisId", luisId);
                }
                else{
                    cmd.Parameters.AddWithValue("@luisId", DBNull.Value);
                }


                

                Debug.WriteLine("query : " + cmd.CommandText);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (rdr.Read())
                {
                    RelationList relationList = new RelationList();
                    relationList.luisId = rdr["LUIS_ID"] as string;
                    relationList.luisIntent = rdr["LUIS_INTENT"] as string;
                    relationList.luisEntities = rdr["LUIS_ENTITIES"] as string;
                    relationList.dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    relationList.dlgApiDefine = rdr["DLG_API_DEFINE"] as string;
                    //relationList.apiId = Convert.ToInt32(rdr["API_ID"] ?? 0);
                    relationList.apiId = rdr["API_ID"].Equals(DBNull.Value)? 0 : Convert.ToInt32(rdr["API_ID"]) ;
                    //DBNull.Value
                    result.Add(relationList);
                }
            }
            return result;
        }
        //KSO END

        //TBL_CHATBOT_CONF 정보 가져오기
        //      LUIS_APP_ID	    - 루이스APP_ID
        //      LUIS_TIME_LIMIT - 루이스제한
        //      LUIS_SCORE_LIMIT - 스코어 제한
        //      LUIS_SUBSCRIPTION   - 루이스구독
        //      BOT_NAME        - 봇이름?
        //      BOT_APP_ID      - 봇앱아이디?
        //      BOT_APP_PASSWORD- 봇앱패스워드?
        //      QUOTE           - 견적url
        //      TESTDRIVE       - 시승url
        //      CATALOG         - 카달로그url
        //      DISCOUNT        - 할인url
        //      EVENT           - 이벤트url

        public List<ConfList> SelectConfig()
        //public List<ConfList> SelectConfig(string config_type)
        {
            SqlDataReader rdr = null;
            List<ConfList> conflist = new List<ConfList>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = " SELECT CNF_TYPE, CNF_NM, CNF_VALUE" +
                                  " FROM TBL_CHATBOT_CONF " +
                                  //" WHERE CNF_TYPE = 'LUIS_APP_ID' " +
                                  " ORDER BY CNF_TYPE DESC, ORDER_NO ASC ";

                Debug.WriteLine("* cmd.CommandText : " + cmd.CommandText);
                //cmd.Parameters.AddWithValue("@config_type", config_type);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    string cnfType = rdr["CNF_TYPE"] as string;
                    string cnfNm = rdr["CNF_NM"] as string;
                    string cnfValue = rdr["CNF_VALUE"] as string;

                    ConfList list = new ConfList();

                    list.cnfType = cnfType;
                    list.cnfNm = cnfNm;
                    list.cnfValue = cnfValue;

                    Debug.WriteLine("* cnfNm : " + cnfNm + " || cnfValue : " + cnfValue);
                    conflist.Add(list);
                }
            }
            return conflist;
        }

		public string SelectChgMsg(string oldMsg)
		{
			SqlDataReader rdr = null;
			string newMsg = "";

			using (SqlConnection conn = new SqlConnection(connStr))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = conn;

				cmd.CommandText += "	SELECT FIND.CHG  CHG_WORD FROM(    					    ";
				cmd.CommandText += "	SELECT                                                  ";
				cmd.CommandText += "			CASE WHEN LEN(ORG_WORD) = LEN(@oldMsg)          ";
				cmd.CommandText += "				THEN CHARINDEX(ORG_WORD, @oldMsg)           ";
				cmd.CommandText += "				ELSE 0                                      ";
				cmd.CommandText += "				END AS FIND_NUM,                            ";
				cmd.CommandText += "				REPLACE(@oldMsg, ORG_WORD, CHG_WORD) CHG    ";
				cmd.CommandText += "	  FROM TBL_WORD_CHG_DICT                                ";
				cmd.CommandText += "	  ) FIND                                                ";
				cmd.CommandText += "	  WHERE FIND.FIND_NUM > 0                               ";





				cmd.Parameters.AddWithValue("@oldMsg", oldMsg);

				rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

				while (rdr.Read())
				{
					newMsg = rdr["CHG_WORD"] as string;
				}
			}
			return newMsg;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// Query Analysis
		// Insert user chat message for history and analysis
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public int insertUserQuery(string korQuery, string intentID, string entitiesIDS, string intentScore, int luisID, char result, int appID)
		{
			int dbResult = 0;
			using (SqlConnection conn = new SqlConnection(connStr))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = conn;
				cmd.CommandText = "sp_insertusehistory4";

				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("@Query", korQuery.Trim().ToLower());
				cmd.Parameters.AddWithValue("@intentID", intentID.Trim());
				cmd.Parameters.AddWithValue("@entitiesIDS", entitiesIDS.Trim().ToLower());
				cmd.Parameters.AddWithValue("@intentScore", intentScore.Trim().ToLower());
				cmd.Parameters.AddWithValue("@luisID", luisID);
				cmd.Parameters.AddWithValue("@result", result);
				cmd.Parameters.AddWithValue("@appID", appID);


				dbResult = cmd.ExecuteNonQuery();
			}
			return dbResult;
		}
		public int insertHistory(string userNumber, string customerCommentKR,  string chatbotCommentCode, string channel, int responseTime, int appID)
		{
			//SqlDataReader rdr = null;
			int result;
			using (SqlConnection conn = new SqlConnection(connStr))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = conn;
				cmd.CommandText += " INSERT INTO TBL_HISTORY_QUERY ";
				cmd.CommandText += " (USER_NUMBER, CUSTOMER_COMMENT_KR, CHATBOT_COMMENT_CODE, CHANNEL, RESPONSE_TIME, REG_DATE, ACTIVE_FLAG, APP_ID) ";
				cmd.CommandText += " VALUES ";
				cmd.CommandText += " (@userNumber, @customerCommentKR, @chatbotCommentCode, @channel, @responseTime, CONVERT(VARCHAR,  GETDATE(), 101) + ' ' + CONVERT(VARCHAR,  DATEADD( HH, 9, GETDATE() ), 24), 0, @appID) ";

				cmd.Parameters.AddWithValue("@userNumber", userNumber);
				cmd.Parameters.AddWithValue("@customerCommentKR", customerCommentKR);
				cmd.Parameters.AddWithValue("@chatbotCommentCode", chatbotCommentCode);
				cmd.Parameters.AddWithValue("@channel", channel);
				cmd.Parameters.AddWithValue("@responseTime", responseTime);
				cmd.Parameters.AddWithValue("@appID", appID);

				result = cmd.ExecuteNonQuery();
				Debug.WriteLine("result : " + result);
			}
			return result;
		}

		public int SelectUserQueryErrorMessageCheck(string userID, int appID)
		{
			SqlDataReader rdr = null;
			int result = 0;
			//userID = arg.Replace("'", "''");
			using (SqlConnection conn = new SqlConnection(connStr))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = conn;

				cmd.CommandText += " SELECT TOP 1 A.CHATBOT_COMMENT_CODE ";
				cmd.CommandText += " FROM ( ";
				cmd.CommandText += " 	SELECT  ";
				cmd.CommandText += " 		SID, ";
				cmd.CommandText += " 		CASE  CHATBOT_COMMENT_CODE  ";
				cmd.CommandText += " 			WHEN 'SEARCH' THEN '1' ";
				cmd.CommandText += " 			WHEN 'ERROR' THEN '1' ";
				cmd.CommandText += " 			ELSE '0' ";
				cmd.CommandText += " 		END CHATBOT_COMMENT_CODE ";
				cmd.CommandText += " 	FROM TBL_HISTORY_QUERY WHERE USER_NUMBER = '" + userID + "' AND APP_ID = " + appID;
				cmd.CommandText += " ) A ";
				cmd.CommandText += " ORDER BY A.SID DESC ";

				rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

				while (rdr.Read())
				{
					result = Convert.ToInt32(rdr["CHATBOT_COMMENT_CODE"]);
				}
			}
			return result;
		}

        public List<TestDriveList_API> SelectTestDriveList_API(String query)
        {
            SqlDataReader rdr = null;

            List<TestDriveList_API> testDriveList_api = new List<TestDriveList_API>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                cmd.CommandText += "    SELECT ENTITY, ENTITY_TYPE, ENTITY_VALUE FROM FN_TESTDRIVEAPI (@query) ";

                cmd.Parameters.AddWithValue("@query", query);

                Debug.WriteLine("query : " + cmd.CommandText);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    //int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    //string dlgIntent = rdr["INTENT"] as string;
                    string dlgEntity = rdr["ENTITY"] as string;
                    string dlgEntityType = rdr["ENTITY_TYPE"] as string;
                    string dlgEntityValue = rdr["ENTITY_VALUE"] as string;

                    TestDriveList_API dlg = new TestDriveList_API();
                    //dlg.intent = dlgIntent;
                    dlg.entity = dlgEntity;
                    dlg.entityType = dlgEntityType;
                    dlg.entityValue = dlgEntityValue;

                    testDriveList_api.Add(dlg);
                }
            }
            return testDriveList_api;
        }
        public List<TestDriveList_API_DLG> SelectTestDriveList_API_DLG(String entitiy_value)
        {
            SqlDataReader rdr = null;

            List<TestDriveList_API_DLG> testDriveList_api_dlg = new List<TestDriveList_API_DLG>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                cmd.CommandText += "   SELECT   A.TESTDRIVE_DLG_ID AS TESTDRIVE_DLG_ID, B.DLG_TYPE AS DLG_TYPE ";
                cmd.CommandText += "   FROM     TBL_TESTDRIVE_RELATION A, TBL_TESTDRIVE_DLG B ";
                cmd.CommandText += "   WHERE    A.TESTDRIVE_DLG_ID = B.TESTDRIVE_DLG_ID ";
                cmd.CommandText += "   AND      MAIN_ENTITY = @entitiy_value ";
                cmd.CommandText += "   ORDER BY DLG_ORDER_NO ";

                cmd.Parameters.AddWithValue("@entitiy_value", entitiy_value);

                Debug.WriteLine("query : " + cmd.CommandText);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int testdrive_dlg_id = Convert.ToInt32(rdr["TESTDRIVE_DLG_ID"]);
                    int dlg_type = Convert.ToInt32(rdr["DLG_TYPE"]);

                    TestDriveList_API_DLG dlg = new TestDriveList_API_DLG();
                    dlg.testdrive_dlg_id = testdrive_dlg_id;
                    dlg.dlg_type = dlg_type;

                    testDriveList_api_dlg.Add(dlg);
                }
            }
            return testDriveList_api_dlg;
        }

        public List<TestDriveList_API_DLG_TEXT> SelectTestDriveList_API_DLG_TEXT(int dlg_id)
        {
            SqlDataReader rdr = null;

            List<TestDriveList_API_DLG_TEXT> testDriveList_api_dlg_text = new List<TestDriveList_API_DLG_TEXT>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                cmd.CommandText += "   SELECT	TEXT_DLG_ID, CARD_TITLE, CARD_TEXT";
                cmd.CommandText += "   FROM     TBL_TESTDRIVE_DLG_TEXT";
                cmd.CommandText += "   WHERE    TESTDRIVE_DLG_ID = @dlg_id";

                cmd.Parameters.AddWithValue("@dlg_id", dlg_id);

                Debug.WriteLine("query : " + cmd.CommandText);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int dlg_text_dlg_id = Convert.ToInt32(rdr["TEXT_DLG_ID"]);
                    string testdrive_card_title = rdr["CARD_TITLE"] as string;
                    string testdrive_card_text = rdr["CARD_TEXT"] as string;

                    TestDriveList_API_DLG_TEXT dlg = new TestDriveList_API_DLG_TEXT();
                    dlg.dlg_text_dlg_id = dlg_text_dlg_id;
                    dlg.testdrive_card_title = testdrive_card_title;
                    dlg.testdrive_card_text = testdrive_card_text;

                    testDriveList_api_dlg_text.Add(dlg);
                }
            }
            return testDriveList_api_dlg_text;
        }


        public List<TestDriveList_API_DLG_MEDIA> SelectTestDriveList_API_DLG_MEDIA(int dlg_id, String entities)
        {
            SqlDataReader rdr = null;

            List<TestDriveList_API_DLG_MEDIA> testDriveList_api_dlg_media = new List<TestDriveList_API_DLG_MEDIA>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                //cmd.CommandText += "    SELECT	GUBUN, ";
                //cmd.CommandText += "        REPLACE(REPLACE(REPLACE(REPLACE(CARD_TITLE,'CALL(모델명)', STR4), 'CALL(지점명)', STR1), 'CALL(지역명)', STR1),'CALL(색상)',STR1) AS CARD_TITLE, ";
                //cmd.CommandText += "        REPLACE(REPLACE(REPLACE(REPLACE(CARD_SUBTITLE,'CALL(색상)', STR7),'CALL(개수)', STR3),'CALL(지점명)', STR2),'CALL(전화번호)', STR3) AS CARD_SUBTITLE, ";
                //cmd.CommandText += "        REPLACE(REPLACE(REPLACE(REPLACE(CARD_TEXT,'CALL(색상)', STR2),'CALL(지역명)', STR1),'CALL(지점 주소)', STR2),'CALL(매장개수)', STR3) AS CARD_TEXT, ";
                //cmd.CommandText += "        REPLACE(REPLACE(MEDIA_URL,'CALL(COLOR_IMG)','https://bot.hyundai.com/assets/images/price/exterior/'+STR2+'.jpg'),'CALL(IMG_URL)','https://bottest.hyundai.com/map/'+STR4+','+STR5+'.png') AS MEDIA_URL, ";
                //cmd.CommandText += "        BTN_1_TYPE, BTN_1_TITLE, REPLACE(REPLACE(REPLACE(BTN_1_CONTEXT, 'CALL(지역명)', STR2),'CALL(색상)', STR1) ,'CALL(지점명)', STR1) AS BTN_1_CONTEXT, ";
                //cmd.CommandText += "        BTN_2_TYPE, BTN_2_TITLE, REPLACE(BTN_2_CONTEXT,'CALL(지점명)', STR1) AS BTN_2_CONTEXT, ";
                //cmd.CommandText += "        BTN_3_TYPE, BTN_3_TITLE, REPLACE(BTN_3_CONTEXT,'CALL(지점명)', STR1) AS BTN_3_CONTEXT, ";
                //cmd.CommandText += "        STR2 AS ADDRESS ";
                //cmd.CommandText += "    FROM FN_LUIS_BRANCH_DRIVE (@entities) A, ";
                //cmd.CommandText += "        TBL_TESTDRIVE_DLG_MEDIA B ";
                //cmd.CommandText += "    WHERE   TESTDRIVE_DLG_ID = @dlg_id ";

                cmd.CommandText = "SELECT GUBUN, CARD_TITLE, CARD_SUBTITLE, CARD_TEXT, MEDIA_URL, BTN_1_TYPE, BTN_1_TITLE, BTN_1_CONTEXT, BTN_2_TYPE, BTN_2_TITLE, BTN_2_CONTEXT, BTN_3_TYPE, BTN_3_TITLE, BTN_3_CONTEXT, ADDRESS FROM FN_TESTDRIVE_MEDIA_NEW(@dlg_id, @entities)";

                cmd.Parameters.AddWithValue("@entities", entities);
                cmd.Parameters.AddWithValue("@dlg_id", dlg_id);

                Debug.WriteLine("query : " + cmd.CommandText);
                Debug.WriteLine("entities : " + entities);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    //int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    string dlgGubun = rdr["GUBUN"] as string;
                    string dlgCardTitle = rdr["CARD_TITLE"] as string;
                    string dlgCardSubTitle = rdr["CARD_SUBTITLE"] as string;
                    string dlgCardText = rdr["CARD_TEXT"] as string;
                    string dlgMediaUrl = rdr["MEDIA_URL"] as string;
                    string dglBtn1Type = rdr["BTN_1_TYPE"] as string;
                    string dlgBtn1Title = rdr["BTN_1_TITLE"] as string;
                    string dlgBtn1Context = rdr["BTN_1_CONTEXT"] as string;
                    string dglBtn2Type = rdr["BTN_2_TYPE"] as string;
                    string dlgBtn2Title = rdr["BTN_2_TITLE"] as string;
                    string dlgBtn2Context = rdr["BTN_2_CONTEXT"] as string;
                    string dglBtn3Type = rdr["BTN_3_TYPE"] as string;
                    string dlgBtn3Title = rdr["BTN_3_TITLE"] as string;
                    string dlgBtn3Context = rdr["BTN_3_CONTEXT"] as string;
                    string dlgAddress = rdr["ADDRESS"] as string;


                    TestDriveList_API_DLG_MEDIA dlg = new TestDriveList_API_DLG_MEDIA();
                    dlg.dlgGubun = dlgGubun;
                    dlg.card_title = dlgCardTitle;
                    dlg.card_subtitle = dlgCardSubTitle;
                    dlg.card_text = dlgCardText;
                    dlg.media_url = dlgMediaUrl;
                    dlg.btn_1_type = dglBtn1Type;
                    dlg.btn_1_title = dlgBtn1Title;
                    dlg.btn_1_context = dlgBtn1Context;
                    dlg.btn_2_type = dglBtn2Type;
                    dlg.btn_2_title = dlgBtn2Title;
                    dlg.btn_2_context = dlgBtn2Context;
                    dlg.btn_3_type = dglBtn3Type;
                    dlg.btn_3_title = dlgBtn3Title;
                    dlg.btn_3_context = dlgBtn3Context;
                    dlg.address = dlgAddress;

                    testDriveList_api_dlg_media.Add(dlg);
                }
            }
            return testDriveList_api_dlg_media;
        }

        public List<KeywordGroup> SelectKeywordGroupList(String[] entities)
        {
            SqlDataReader rdr = null;

            List<KeywordGroup> keywordgrouplist = new List<KeywordGroup>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                cmd.CommandText += "SELECT KEYWORD, KEYWORDGROUP FROM TBL_KONA_PRICE_KEYWORD WHERE KEYWORD = '" + entities[0].Replace(" ", "") + "'";
                 
                for (int i = 1; i < entities.Length; i++)
                {
                    cmd.CommandText += "OR KEYWORD = '" + entities[i].Replace(" ", "") + "'";
                }

                Debug.WriteLine("price keyword group query : " + cmd.CommandText);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    KeywordGroup keywordgroup = new KeywordGroup();
                    keywordgroup.keyword = rdr["KEYWORD"] as string;
                    keywordgroup.keywordgroup = rdr["KEYWORDGROUP"] as string;
                    keywordgrouplist.Add(keywordgroup);
                }
            }
            return keywordgrouplist;
        }

        public string SelectedRecommendConfirm(string kr_query)
        {
            SqlDataReader rdr = null;

            string dlgKeywordgroup = "";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                cmd.CommandText += "SELECT	KEYWORD, KEYWORDGROUP ";
                cmd.CommandText += "FROM TBL_RECOMMEND_KEYWORD ";
                cmd.CommandText += "WHERE CHARINDEX(KEYWORD,@kr_query) > 0";

                cmd.Parameters.AddWithValue("@kr_query", kr_query);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                try
                {
                    while (rdr.Read())
                    {
                        //string dlgKeyword = rdr["KEYWORD"] as string;
                        dlgKeywordgroup = rdr["KEYWORDGROUP"] as string;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return dlgKeywordgroup;
        }

        public int SelectedRecommendDlgId(string recommendValue)
        {
            SqlDataReader rdr = null;

            int dlgRecommendDlgId = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                cmd.CommandText += "SELECT  TOP 1 RECOMMEND_DLG_ID ";
                cmd.CommandText += "FROM    TBL_RECOMMEND_RELATION ";
                cmd.CommandText += "WHERE   ENTITY = @recommendValue ";

                cmd.Parameters.AddWithValue("@recommendValue", recommendValue);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    dlgRecommendDlgId = Convert.ToInt32(rdr["RECOMMEND_DLG_ID"]);

                    //RecommendDialogId dlg = new RecommendDialogId();
                    //dlg.dlgId = dlgRecommendDlgId;

                    //testDriveList_api_dlg_media.Add(dlg);
                }
            }
            return dlgRecommendDlgId;
        }

        public List<Recommend_DLG_MEDIA> SelectRecommend_DLG_MEDIA(int dlg_id)
        {
            SqlDataReader rdr = null;

            List<Recommend_DLG_MEDIA> recommend_dlg_media = new List<Recommend_DLG_MEDIA>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                cmd.CommandText += "    SELECT	CARD_TITLE, CARD_TEXT, MEDIA_URL, ";
                cmd.CommandText += "            BTN_1_TYPE, BTN_1_TITLE, BTN_1_CONTEXT, ";
                cmd.CommandText += "            BTN_2_TYPE, BTN_2_TITLE, BTN_2_CONTEXT, ";
                cmd.CommandText += "            BTN_3_TYPE, BTN_3_TITLE, BTN_3_CONTEXT, ";
                cmd.CommandText += "            BTN_4_TYPE, BTN_4_TITLE, BTN_4_CONTEXT, ";
                cmd.CommandText += "            BTN_5_TYPE, BTN_5_TITLE, BTN_5_CONTEXT ";
                cmd.CommandText += "    FROM    TBL_RECOMMEND_DLG_MEDIA ";
                cmd.CommandText += "    WHERE   RECOMMEND_DLG_ID = @dlg_id ";

                cmd.Parameters.AddWithValue("@dlg_id", dlg_id);

                Debug.WriteLine("query : " + cmd.CommandText);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    string dlgCardTitle = rdr["CARD_TITLE"] as string;
                    //string dlgCardSubTitle = rdr["CARD_SUBTITLE"] as string;
                    string dlgCardText = rdr["CARD_TEXT"] as string;
                    string dlgMediaUrl = rdr["MEDIA_URL"] as string;
                    string dglBtn1Type = rdr["BTN_1_TYPE"] as string;
                    string dlgBtn1Title = rdr["BTN_1_TITLE"] as string;
                    string dlgBtn1Context = rdr["BTN_1_CONTEXT"] as string;
                    string dglBtn2Type = rdr["BTN_2_TYPE"] as string;
                    string dlgBtn2Title = rdr["BTN_2_TITLE"] as string;
                    string dlgBtn2Context = rdr["BTN_2_CONTEXT"] as string;
                    string dglBtn3Type = rdr["BTN_3_TYPE"] as string;
                    string dlgBtn3Title = rdr["BTN_3_TITLE"] as string;
                    string dlgBtn3Context = rdr["BTN_3_CONTEXT"] as string;
                    string dglBtn4Type = rdr["BTN_4_TYPE"] as string;
                    string dlgBtn4Title = rdr["BTN_4_TITLE"] as string;
                    string dlgBtn4Context = rdr["BTN_4_CONTEXT"] as string;
                    string dglBtn5Type = rdr["BTN_5_TYPE"] as string;
                    string dlgBtn5Title = rdr["BTN_5_TITLE"] as string;
                    string dlgBtn5Context = rdr["BTN_5_CONTEXT"] as string;

                    Recommend_DLG_MEDIA dlg = new Recommend_DLG_MEDIA();
                    dlg.card_title = dlgCardTitle;
                    //dlg.card_subtitle = dlgCardSubTitle;
                    dlg.card_text = dlgCardText;
                    dlg.media_url = dlgMediaUrl;
                    dlg.btn_1_type = dglBtn1Type;
                    dlg.btn_1_title = dlgBtn1Title;
                    dlg.btn_1_context = dlgBtn1Context;
                    dlg.btn_2_type = dglBtn2Type;
                    dlg.btn_2_title = dlgBtn2Title;
                    dlg.btn_2_context = dlgBtn2Context;
                    dlg.btn_3_type = dglBtn3Type;
                    dlg.btn_3_title = dlgBtn3Title;
                    dlg.btn_3_context = dlgBtn3Context;
                    dlg.btn_4_type = dglBtn4Type;
                    dlg.btn_4_title = dlgBtn4Title;
                    dlg.btn_4_context = dlgBtn4Context;
                    dlg.btn_5_type = dglBtn5Type;
                    dlg.btn_5_title = dlgBtn5Title;
                    dlg.btn_5_context = dlgBtn5Context;

                    recommend_dlg_media.Add(dlg);
                }
            }
            return recommend_dlg_media;
        }

        public List<RecommendList> SelectedRecommendList(string usage, string importance, string gender, string age)
        {
            SqlDataReader rdr = null;
            List<RecommendList> recommendList = new List<RecommendList>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                cmd.CommandText += "SELECT TOP 1 RECOMMEND_TITLE, ANSWER_1, ANSWER_2, ANSWER_3, TRIM_DETAIL, TRIM_DETAIL_PRICE, ";
                cmd.CommandText += "REPLACE(OPTION_1,'추가','@추가') AS OPTION_1, OPTION_1_IMG_URL, ";
                cmd.CommandText += "REPLACE(OPTION_2,'추가','@추가') AS OPTION_2, OPTION_2_IMG_URL, ";
                cmd.CommandText += "REPLACE(OPTION_3,'추가','@추가') AS OPTION_3, OPTION_3_IMG_URL, ";
                cmd.CommandText += "REPLACE(OPTION_4,'추가','@추가') AS OPTION_4, OPTION_4_IMG_URL, ";
                cmd.CommandText += "OPTION_5, OPTION_5_IMG_URL, ";
                cmd.CommandText += "MAIN_COLOR_VIEW_1, MAIN_COLOR_VIEW_2, MAIN_COLOR_VIEW_3, MAIN_COLOR_VIEW_4, MAIN_COLOR_VIEW_5, MAIN_COLOR_VIEW_6, MAIN_COLOR_VIEW_7, ";
                cmd.CommandText += "REPLACE(MAIN_COLOR_VIEW_NM1,'@@','') AS MAIN_COLOR_VIEW_NM1, ";
                cmd.CommandText += "REPLACE(MAIN_COLOR_VIEW_NM2,'@@','') AS MAIN_COLOR_VIEW_NM2, ";
                cmd.CommandText += "REPLACE(MAIN_COLOR_VIEW_NM3,'@@','') AS MAIN_COLOR_VIEW_NM3, ";
                cmd.CommandText += "REPLACE(MAIN_COLOR_VIEW_NM4,'@@','') AS MAIN_COLOR_VIEW_NM4, ";
                cmd.CommandText += "REPLACE(MAIN_COLOR_VIEW_NM5,'@@','') AS MAIN_COLOR_VIEW_NM5, ";
                cmd.CommandText += "REPLACE(MAIN_COLOR_VIEW_NM6,'@@','') AS MAIN_COLOR_VIEW_NM6, ";
                cmd.CommandText += "REPLACE(MAIN_COLOR_VIEW_NM7,'@@','') AS MAIN_COLOR_VIEW_NM7 ";
                cmd.CommandText += "FROM ";
                cmd.CommandText += "    ( ";
                cmd.CommandText += "    SELECT  RECOMMEND_TITLE, ANSWER_1, ANSWER_2, ANSWER_3, ";
                cmd.CommandText += "             LEFT(TRIM_DETAIL, CHARINDEX('[', TRIM_DETAIL) - 2) AS TRIM_DETAIL, ";
                cmd.CommandText += "               SUBSTRING(TRIM_DETAIL, CHARINDEX('[', TRIM_DETAIL)+1, (CHARINDEX('/', TRIM_DETAIL)) - (CHARINDEX('[', TRIM_DETAIL) + 1)) AS TRIM_DETAIL_PRICE ";
                cmd.CommandText += "              , OPTION_1, OPTION_1_IMG_URL, OPTION_2, OPTION_2_IMG_URL, OPTION_3, OPTION_3_IMG_URL, OPTION_4, OPTION_4_IMG_URL, OPTION_5, OPTION_5_IMG_URL, ";
                cmd.CommandText += "            OPTION_6, OPTION_6_IMG_URL,  ";
                cmd.CommandText += "            (SELECT  LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1) AS TRIMCOLOR_CD FROM TBL_TRIMCOLOR2 WHERE TRIMCOLOR_NM = LEFT(MAIN_COLOR_VIEW_1,CHARINDEX('/',MAIN_COLOR_VIEW_1)-1) GROUP BY LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1)) AS MAIN_COLOR_VIEW_1,  ";
                cmd.CommandText += "            (SELECT  LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1) AS TRIMCOLOR_CD FROM TBL_TRIMCOLOR2 WHERE TRIMCOLOR_NM = LEFT(MAIN_COLOR_VIEW_2,CHARINDEX('/',MAIN_COLOR_VIEW_2)-1) GROUP BY LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1)) AS MAIN_COLOR_VIEW_2,  ";
                cmd.CommandText += "            (SELECT  LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1) AS TRIMCOLOR_CD FROM TBL_TRIMCOLOR2 WHERE TRIMCOLOR_NM = LEFT(MAIN_COLOR_VIEW_3,CHARINDEX('/',MAIN_COLOR_VIEW_3)-1) GROUP BY LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1)) AS MAIN_COLOR_VIEW_3,  ";
                cmd.CommandText += "            (SELECT  LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1) AS TRIMCOLOR_CD FROM TBL_TRIMCOLOR2 WHERE TRIMCOLOR_NM = LEFT(MAIN_COLOR_VIEW_4,CHARINDEX('/',MAIN_COLOR_VIEW_4)-1) GROUP BY LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1)) AS MAIN_COLOR_VIEW_4,  ";
                cmd.CommandText += "            (SELECT  LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1) AS TRIMCOLOR_CD FROM TBL_TRIMCOLOR2 WHERE TRIMCOLOR_NM = LEFT(MAIN_COLOR_VIEW_5,CHARINDEX('/',MAIN_COLOR_VIEW_5)-1) GROUP BY LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1)) AS MAIN_COLOR_VIEW_5,  ";
                cmd.CommandText += "            (SELECT  LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1) AS TRIMCOLOR_CD FROM TBL_TRIMCOLOR2 WHERE TRIMCOLOR_NM = LEFT(MAIN_COLOR_VIEW_6,CHARINDEX('/',MAIN_COLOR_VIEW_6)-1) GROUP BY LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1)) AS MAIN_COLOR_VIEW_6,  ";
                cmd.CommandText += "            (SELECT  LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1) AS TRIMCOLOR_CD FROM TBL_TRIMCOLOR2 WHERE TRIMCOLOR_NM = LEFT(MAIN_COLOR_VIEW_7,CHARINDEX('/',MAIN_COLOR_VIEW_7)-1) GROUP BY LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1)) AS MAIN_COLOR_VIEW_7,  ";
                cmd.CommandText += "            LEFT(MAIN_COLOR_VIEW_1, CHARINDEX('/', MAIN_COLOR_VIEW_1) - 1) AS MAIN_COLOR_VIEW_NM1, ";
                cmd.CommandText += "			LEFT(MAIN_COLOR_VIEW_2, CHARINDEX('/', MAIN_COLOR_VIEW_2) - 1) AS MAIN_COLOR_VIEW_NM2, ";
                cmd.CommandText += "			LEFT(MAIN_COLOR_VIEW_3, CHARINDEX('/', MAIN_COLOR_VIEW_3) - 1) AS MAIN_COLOR_VIEW_NM3, ";
                cmd.CommandText += "			LEFT(MAIN_COLOR_VIEW_4, CHARINDEX('/', MAIN_COLOR_VIEW_4) - 1) AS MAIN_COLOR_VIEW_NM4, ";
                cmd.CommandText += "			LEFT(MAIN_COLOR_VIEW_5, CHARINDEX('/', MAIN_COLOR_VIEW_5) - 1) AS MAIN_COLOR_VIEW_NM5, ";
                cmd.CommandText += "			LEFT(MAIN_COLOR_VIEW_6, CHARINDEX('/', MAIN_COLOR_VIEW_6) - 1) AS MAIN_COLOR_VIEW_NM6, ";
                cmd.CommandText += "			LEFT(MAIN_COLOR_VIEW_7, CHARINDEX('/', MAIN_COLOR_VIEW_7) - 1) AS MAIN_COLOR_VIEW_NM7  ";
                cmd.CommandText += "    FROM    TBL_RECOMMEND_TRIM ";
                cmd.CommandText += "    WHERE   ANSWER_1 = CASE WHEN CHARINDEX('주말', @usage) > 0 OR CHARINDEX('레저', @usage) > 0 OR CHARINDEX('레져', @usage) > 0 OR CHARINDEX('장거리', @usage) > 0 THEN '장거리' ";
                cmd.CommandText += "						ELSE '출퇴근' END ";
                cmd.CommandText += "    AND     ANSWER_2 = @importance ";
                cmd.CommandText += "    AND     ANSWER_3 = CASE WHEN CHARINDEX('여성', @gender) > 0 OR CHARINDEX('여자', @gender) > 0 OR CHARINDEX('여', @gender) > 0 OR CHARINDEX('female', @gender) > 0 OR CHARINDEX('woman', @gender) > 0 OR CHARINDEX('women', @gender) > 0 OR CHARINDEX('girl', @gender) > 0 THEN '여성' ";
                cmd.CommandText += "                            WHEN CHARINDEX('남성', @gender) > 0 OR CHARINDEX('남자', @gender) > 0 OR CHARINDEX('남', @gender) > 0 OR CHARINDEX('male', @gender) > 0 OR CHARINDEX('man', @gender) > 0 OR CHARINDEX('men', @gender) > 0 OR CHARINDEX('boy', @gender) > 0 THEN '남성' ";
                cmd.CommandText += "                            ELSE '기타' END ";
                cmd.CommandText += "    UNION ALL ";
                cmd.CommandText += "    SELECT  RECOMMEND_TITLE, ANSWER_1, ANSWER_2, ANSWER_3, ";
                cmd.CommandText += "             LEFT(TRIM_DETAIL, CHARINDEX('[', TRIM_DETAIL) - 2) AS TRIM_DETAIL, ";
                cmd.CommandText += "               SUBSTRING(TRIM_DETAIL, CHARINDEX('[', TRIM_DETAIL)+1, (CHARINDEX('/', TRIM_DETAIL)) - (CHARINDEX('[', TRIM_DETAIL) + 1)) AS TRIM_DETAIL_PRICE ";
                cmd.CommandText += "              , OPTION_1, OPTION_1_IMG_URL, OPTION_2, OPTION_2_IMG_URL, OPTION_3, OPTION_3_IMG_URL, OPTION_4, OPTION_4_IMG_URL, OPTION_5, OPTION_5_IMG_URL, ";
                cmd.CommandText += "            OPTION_6, OPTION_6_IMG_URL,  ";
                cmd.CommandText += "            (SELECT  LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1) AS TRIMCOLOR_CD FROM TBL_TRIMCOLOR2 WHERE TRIMCOLOR_NM = LEFT(MAIN_COLOR_VIEW_1,CHARINDEX('/',MAIN_COLOR_VIEW_1)-1) GROUP BY LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1)) AS MAIN_COLOR_VIEW_1,  ";
                cmd.CommandText += "            (SELECT  LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1) AS TRIMCOLOR_CD FROM TBL_TRIMCOLOR2 WHERE TRIMCOLOR_NM = LEFT(MAIN_COLOR_VIEW_2,CHARINDEX('/',MAIN_COLOR_VIEW_2)-1) GROUP BY LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1)) AS MAIN_COLOR_VIEW_2,  ";
                cmd.CommandText += "            (SELECT  LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1) AS TRIMCOLOR_CD FROM TBL_TRIMCOLOR2 WHERE TRIMCOLOR_NM = LEFT(MAIN_COLOR_VIEW_3,CHARINDEX('/',MAIN_COLOR_VIEW_3)-1) GROUP BY LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1)) AS MAIN_COLOR_VIEW_3,  ";
                cmd.CommandText += "            (SELECT  LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1) AS TRIMCOLOR_CD FROM TBL_TRIMCOLOR2 WHERE TRIMCOLOR_NM = LEFT(MAIN_COLOR_VIEW_4,CHARINDEX('/',MAIN_COLOR_VIEW_4)-1) GROUP BY LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1)) AS MAIN_COLOR_VIEW_4,  ";
                cmd.CommandText += "            (SELECT  LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1) AS TRIMCOLOR_CD FROM TBL_TRIMCOLOR2 WHERE TRIMCOLOR_NM = LEFT(MAIN_COLOR_VIEW_5,CHARINDEX('/',MAIN_COLOR_VIEW_5)-1) GROUP BY LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1)) AS MAIN_COLOR_VIEW_5,  ";
                cmd.CommandText += "            (SELECT  LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1) AS TRIMCOLOR_CD FROM TBL_TRIMCOLOR2 WHERE TRIMCOLOR_NM = LEFT(MAIN_COLOR_VIEW_6,CHARINDEX('/',MAIN_COLOR_VIEW_6)-1) GROUP BY LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1)) AS MAIN_COLOR_VIEW_6,  ";
                cmd.CommandText += "            (SELECT  LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1) AS TRIMCOLOR_CD FROM TBL_TRIMCOLOR2 WHERE TRIMCOLOR_NM = LEFT(MAIN_COLOR_VIEW_7,CHARINDEX('/',MAIN_COLOR_VIEW_7)-1) GROUP BY LEFT(TRIMCOLOR_CD, CHARINDEX(':',TRIMCOLOR_CD)-1)) AS MAIN_COLOR_VIEW_7,  ";
                cmd.CommandText += "            LEFT(MAIN_COLOR_VIEW_1, CHARINDEX('/', MAIN_COLOR_VIEW_1) - 1) AS MAIN_COLOR_VIEW_NM1, ";
                cmd.CommandText += "			LEFT(MAIN_COLOR_VIEW_2, CHARINDEX('/', MAIN_COLOR_VIEW_2) - 1) AS MAIN_COLOR_VIEW_NM2, ";
                cmd.CommandText += "			LEFT(MAIN_COLOR_VIEW_3, CHARINDEX('/', MAIN_COLOR_VIEW_3) - 1) AS MAIN_COLOR_VIEW_NM3, ";
                cmd.CommandText += "			LEFT(MAIN_COLOR_VIEW_4, CHARINDEX('/', MAIN_COLOR_VIEW_4) - 1) AS MAIN_COLOR_VIEW_NM4, ";
                cmd.CommandText += "			LEFT(MAIN_COLOR_VIEW_5, CHARINDEX('/', MAIN_COLOR_VIEW_5) - 1) AS MAIN_COLOR_VIEW_NM5, ";
                cmd.CommandText += "			LEFT(MAIN_COLOR_VIEW_6, CHARINDEX('/', MAIN_COLOR_VIEW_6) - 1) AS MAIN_COLOR_VIEW_NM6, ";
                cmd.CommandText += "			LEFT(MAIN_COLOR_VIEW_7, CHARINDEX('/', MAIN_COLOR_VIEW_7) - 1) AS MAIN_COLOR_VIEW_NM7  ";
                cmd.CommandText += "    FROM    TBL_RECOMMEND_TRIM ";
                cmd.CommandText += "    WHERE   ANSWER_1 = '기타' ";
                cmd.CommandText += ") A ";

                cmd.Parameters.AddWithValue("@usage", usage);
                cmd.Parameters.AddWithValue("@importance", importance);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@age", age);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (rdr.Read())
                {
                    RecommendList dlg = new RecommendList();
                    dlg.RECOMMEND_TITLE = rdr["RECOMMEND_TITLE"] as string;
                    dlg.TRIM_DETAIL = rdr["TRIM_DETAIL"] as string;
                    dlg.TRIM_DETAIL_PRICE = rdr["TRIM_DETAIL_PRICE"] as string;
                    dlg.ANSWER_1 = rdr["ANSWER_1"] as string;
                    dlg.ANSWER_2 = rdr["ANSWER_2"] as string;
                    dlg.ANSWER_3 = rdr["ANSWER_3"] as string;
                    dlg.OPTION_1 = rdr["OPTION_1"] as string;
                    dlg.OPTION_1_IMG_URL = rdr["OPTION_1_IMG_URL"] as string;
                    dlg.OPTION_2 = rdr["OPTION_2"] as string;
                    dlg.OPTION_2_IMG_URL = rdr["OPTION_2_IMG_URL"] as string;
                    dlg.OPTION_3 = rdr["OPTION_3"] as string;
                    dlg.OPTION_3_IMG_URL = rdr["OPTION_3_IMG_URL"] as string;
                    dlg.OPTION_4 = rdr["OPTION_4"] as string;
                    dlg.OPTION_4_IMG_URL = rdr["OPTION_4_IMG_URL"] as string;
                    dlg.OPTION_5 = rdr["OPTION_5"] as string;
                    dlg.OPTION_5_IMG_URL = rdr["OPTION_5_IMG_URL"] as string;
                    //dlg.OPTION_6 = rdr["OPTION_6"] as string;
                    //dlg.OPTION_6_IMG_URL = rdr["OPTION_6_IMG_URL"] as string;
                    dlg.MAIN_COLOR_VIEW_1 = rdr["MAIN_COLOR_VIEW_1"] as string;
                    dlg.MAIN_COLOR_VIEW_2 = rdr["MAIN_COLOR_VIEW_2"] as string;
                    dlg.MAIN_COLOR_VIEW_3 = rdr["MAIN_COLOR_VIEW_3"] as string;
                    dlg.MAIN_COLOR_VIEW_4 = rdr["MAIN_COLOR_VIEW_4"] as string;
                    dlg.MAIN_COLOR_VIEW_5 = rdr["MAIN_COLOR_VIEW_5"] as string;
                    dlg.MAIN_COLOR_VIEW_6 = rdr["MAIN_COLOR_VIEW_6"] as string;
                    dlg.MAIN_COLOR_VIEW_7 = rdr["MAIN_COLOR_VIEW_7"] as string;
                    dlg.MAIN_COLOR_VIEW_NM1 = rdr["MAIN_COLOR_VIEW_NM1"] as string;
                    dlg.MAIN_COLOR_VIEW_NM2 = rdr["MAIN_COLOR_VIEW_NM2"] as string;
                    dlg.MAIN_COLOR_VIEW_NM3 = rdr["MAIN_COLOR_VIEW_NM3"] as string;
                    dlg.MAIN_COLOR_VIEW_NM4 = rdr["MAIN_COLOR_VIEW_NM4"] as string;
                    dlg.MAIN_COLOR_VIEW_NM5 = rdr["MAIN_COLOR_VIEW_NM5"] as string;
                    dlg.MAIN_COLOR_VIEW_NM6 = rdr["MAIN_COLOR_VIEW_NM6"] as string;
                    dlg.MAIN_COLOR_VIEW_NM7 = rdr["MAIN_COLOR_VIEW_NM7"] as string;

                    recommendList.Add(dlg);
                }

            }
            return recommendList;
        }

        public string LocationValue(string arg1, string arg2)
        {
            SqlDataReader rdr = null;
            string result = "";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                cmd.CommandText += "SELECT TOP 1 BR_DTL_ADDR1 ";
                cmd.CommandText += "FROM    ";
                cmd.CommandText += "    (   ";
                cmd.CommandText += "        SELECT dbo.FN_TO_DISTANCE(@arg1, @arg2, BR_XCOO, BR_YCOO) AS DISTANCE, BR_DTL_ADDR1    ";
                cmd.CommandText += "        FROM TBL_BR_MGMT ";
                cmd.CommandText += "    ) A ";
                cmd.CommandText += "ORDER BY A.DISTANCE     ";

                cmd.Parameters.AddWithValue("@arg1", arg1);
                cmd.Parameters.AddWithValue("@arg2", arg2);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    result = rdr["BR_DTL_ADDR1"] as string;
                }
            }
            return result;
        }
    }
}