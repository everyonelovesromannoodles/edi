Imports System.IO
Imports System.Net



Module Module1

    Sub Main()
        Dim app2 As String



        app2 = Appstart()

        Dim cni As Long
        Dim cni2 As Long = 0
        Dim lx1% = 0

        cni = Constart()
        Dim sndrpt As String = "EDI Invoice Report For: " & CStr(Format(Now, "MM/dd/yy")) & vbNewLine & vbNewLine
        GEN(cni, cni2, lx1%, sndrpt)

        If lx1% > 0 Then
            Conend(cni)

        End If
        lx1% = 0 : cni2 = 0
        sndrpt = sndrpt & vbNewLine & vbNewLine
        LGEN(cni, cni2, lx1%, sndrpt)


        If lx1% > 0 Then
            Conend(cni)

        End If

        If Val(app2) > 0 Then GoTo FEND
        Append(0)

FEND:

VV99:
    End Sub

    Sub ISAJC(ByRef cni2 As Long, ByRef cni As Long, ByRef seg As Long, ByVal po2 As String,
          ByVal invdt8 As String, ByRef fs2 As StreamWriter,
          ByVal inv As String, ByVal po1 As String, ByRef dt8 As String)
        cni2 = cni2 + 1
        Dim i6 As String = ""
        Dim dt6 As String
        Dim phn As String
        Dim l2 As String
        Dim l3 As String
        Dim t4 As String
        dt6 = Format(Now, "yyMMdd") : t4 = Format(Now, "hhmm")
        dt8 = invdt8
        If cni2 > 1 Then GoTo ISAJCSKIP
        cni = cni + 1
        'phn = "0000      "  TEST
        ' phn = "0000  " ' LIVE
        phn = "0000" ' new live

        l2 = phn & "*" & dt6 & "*" & t4
        l3 = "*U*00401*" & Format(cni, "000000000") & "*0*P*^" & i6
        'LIVE
        fs2.WriteLine("ISA*00*          *00*          *01*SPECIAL_CODE      *09*" & l2 & l3)
        'TEST Print #2, "ISA*00*          *00*          *01*SPECIAL_CODE      *01*" & l2 & l3

        fs2.WriteLine("GS*IN*SPECIAL_CODE*SPECIAL_CODE*" & dt8 & "*" & t4 & "*" &
            Format(cni, "########0") & "*X*004010" & i6)

ISAJCSKIP:
        fs2.WriteLine("ST*810*" & Format(cni2, "000000000") & i6)
        seg = seg + 1

        fs2.WriteLine("BIG*" & dt8 & "*" & inv & "**" & po1 & "***DR" & i6)
        seg = seg + 1

        fs2.WriteLine("CUR*SE*USD" & i6) : seg = seg + 1
        If Len(po2) > 0 Then fs2.WriteLine("REF*JB*" & po2 & i6) : seg = seg + 1
    End Sub

    Sub ISAJCL(ByRef cni2 As Long, ByRef cni As Long, ByRef seg As Long, ByVal po2 As String,
          ByVal invdt8 As String, ByRef fs2 As StreamWriter,
          ByVal inv As String, ByVal po1 As String, ByRef dt8 As String)
        cni2 = cni2 + 1
        Dim i6 As String = ""
        Dim dt6 As String
        Dim phn As String
        Dim l2 As String
        Dim l3 As String
        Dim t4 As String
        dt6 = Format(Now, "yyMMdd") : t4 = Format(Now, "hhmm")
        dt8 = invdt8
        If cni2 > 1 Then GoTo ISAJCSKIP
        cni = cni + 1
        'phn = "0000      "  TEST
        phn = "0000  " ' LIVE
        '  phn = "0000"

        l2 = phn & "*" & dt6 & "*" & t4
        l3 = "*U*00401*" & Format(cni, "000000000") & "*0*P*^" & i6
        'LIVE
        fs2.WriteLine("ISA*00*          *00*          *01*SPECIAL_CODE      *09*" & l2 & l3)
        'TEST Print #2, "ISA*00*          *00*          *01*SPECIAL_CODE      *01*" & l2 & l3

        fs2.WriteLine("GS*IN*SPECIAL_CODE*SPECIAL_CODE*" & dt8 & "*" & t4 & "*" &
            Format(cni, "########0") & "*X*004010" & i6)

ISAJCSKIP:
        fs2.WriteLine("ST*810*" & Format(cni2, "000000000") & i6)
        seg = seg + 1

        fs2.WriteLine("BIG*" & dt8 & "*" & inv & "**" & po1 & "***DR" & i6)
        seg = seg + 1

        fs2.WriteLine("CUR*SE*USD" & i6) : seg = seg + 1
        If Len(po2) > 0 Then fs2.WriteLine("REF*JB*" & po2 & i6) : seg = seg + 1
    End Sub
    Sub SAVACK(ByVal cni As Long, ByVal cni2 As Long, ByVal inv As String,
           ByVal custnum1 As Long, ByVal custcd1 As String)
        Dim sSQL As String

        Dim cn As ADODB.Connection
        Dim rs As ADODB.Recordset
        cn = New ADODB.Connection
        rs = New ADODB.Recordset

        cn.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;Data Source=C:\edata\sx64.accdb;
Persist Security Info=False;"
        cn.Open()

        sSQL = "INSERT INTO akinv(SEQNO, CNINO, INVNO, INVDATE, PROCESSTIME, CUSTCD, CUSTNUM) VALUES " &
    "('" & CStr(cni) & "', '" & CStr(cni2) & "', '" & inv & "' ,'" & Now.ToString("d") & "', '" &
     Now.ToString("T") & "', '" & custcd1 & "', '" & custnum1 & "')"
        rs.CursorType = ADODB.CursorTypeEnum.adOpenKeyset
        rs = cn.Execute(sSQL)
        cn.Close()
        cn = Nothing
        rs = Nothing
    End Sub
    Sub GEN(ByRef cni As Long, ByRef cni2 As Long, ByRef lx1%, ByVal sndrpt As String)

        Dim cn2 As ADODB.Connection
        Dim rs2 As ADODB.Recordset
        rs2 = New ADODB.Recordset
        cn2 = New ADODB.Connection
        cn2 = CreateObject("ADODB.Connection")
        cn2.Open("con", "user", "p")

        Dim dt1 As Date = Now
        Dim wf As String
        Dim ck% = 0
        Dim i6 As String = ""
        Dim seg As Long = 0
        Dim b As String
        Dim b2 As String
        Dim dt8 As String = ""
        Dim s As String
        Dim sr As String
        Dim po As String = ""
        Dim inv As String = ""
        Dim qty As Long
        Dim pr As Double
        Dim fx%
        Dim it As String
        Dim desc As String
        Dim po1 As String = ""
        Dim po2 As String = ""
        Dim task As String
        Dim wrko As String = ""
        Dim jobn As String = ""
        Dim shipd As String = ""
        Dim refer As String = ""
        Dim invdt8 As String
        Dim invdt As String
        Dim custnum1 As Long
        Dim nm As String
        Dim sql1 As String
        Dim addr As String
        Dim addr2 As String
        Dim city As String
        Dim sstate As String
        Dim tot As Double
        Dim zip As String
        Dim custcd1 As String = "CUST_DL"
        Dim cusn As String
        cusn = "CUST"
        Dim strtot As String

        Dim sv%
        Dim p7%
        Dim p8%
        Dim p9%
        Dim ttax As Double
        Dim frt As Double
        Dim iti As Long
        Dim ht As Long
        Dim fso As FileInfo
        Dim fsz% = 0

        Dim fshist As New StreamWriter("c:\edi\cust_dl_hist.txt", True)
        wf = Dir("e:\apps\custs\*.*")
        While String.IsNullOrEmpty(wf) = False

            ' Check for data to process.  If not then end.
            fso = New FileInfo("e:\apps\cust\" & wf)
            If fso.Length > 0 Then fsz% = fsz% + 1 Else File.Delete("e:\apps\cust\" & wf)

            wf = Dir()
        End While
        If fsz% = 0 Then fshist.Close() : GoTo CUST599

        Dim fnamep1 As String = "ej" & Format(Now, "yyMMdd") & Format(Now, "hhmmff") & ".txt"
        Dim fnamep As String = "c:\edi\sxout\" & fnamep1
        Dim fs1
        Dim fs2 As New StreamWriter(fnamep, False)
        wf = Dir("e:\apps\cust\*.*")

        While String.IsNullOrEmpty(wf) = False
            fs1 = New StreamReader("e:\apps\cust\" & wf)

            Do While fs1.peek >= 0
RR4:
                b = fs1.readline


                b2 = CStr(b)
                s = Trim(Left(b, 6).ToString)

                If s = "Inv" Then

                    sr = md(b2, 24, 22)
                    po = Trim(sr)
                    sr = md(b2, 619, 8)
                    inv = sr
                    inv = inv & "-" & md(b2, 22, 2)

                    CUSTScreen(fs1, b2, ck%)
                    If ck% = 1 Then
                        Dim ps1 As StreamWriter = New StreamWriter("c:\edi\cust.txt", True)
                        ps1.WriteLine(CStr(Format(Now, "MM/dd/yy")) & " " & po & "  " & inv)
                        ps1.Close()
                        ps1 = Nothing
                        GoTo RR4
                    End If

                    b = b2
                    sv% = 0
                    iti = 0 : ht = 0 : frt = 0

                    sr = md(b2, 7, 8)
                    dt1 = Date.Parse(sr)
                    invdt8 = Format(dt1, "yyyyMMdd")
                    invdt = Format(dt1, "yyMMdd")

                    p9% = InStr(1, po, "V#")
                    If p9% > 0 Then
                        p8% = InStr(1, po, " ")
                        If p8% > 0 Then
                            p7% = Len(po)
                            po1 = Mid(po, 1, p8% - 1)
                            po2 = Mid(po, p8% + 1, p7% - p8%)
                        End If
                    Else
                        po1 = po : po2 = ""
                    End If

                    sr = md(b2, 58, 3)
                    refer = Trim(sr)
                    If refer = "EDI" Then refer = ""
                    sr = md(b2, 61, 21)
                    task = Trim(sr)
                    sr = md(b2, 592, 10)
                    wrko = Trim(sr)
                    sr = md(b2, 562, 30)
                    jobn = Trim(sr)
                    sr = md(b2, 147, 13)
                    ttax = Val(sr)
                    sr = md(b2, 306, 24)
                    shipd = Trim(sr)
                    ISAJC(cni2, cni, seg, po2, invdt8, fs2, inv, po1, dt8)
                    lx1% = lx1% + 1
                    'GoSub SAVACK
                    If task <> "" Then
                        fs2.WriteLine("REF*K5*" & task & i6) : seg = seg + 1
                    End If

                    If wrko <> "" Then
                        fs2.WriteLine("REF*WO*" & wrko & i6) : seg = seg + 1
                    End If
                    fs2.WriteLine("PER*BI*" & jobn & i6) : seg = seg + 1
                    fs2.WriteLine("PER*OD*" & refer & i6) : seg = seg + 1
                End If
                If s = "Custom" Then
                    sr = md(b2, 165, 10) : custnum1 = Val(sr)
                    SAVACK(cni, cni2, inv, custnum1, custcd1)
                    sv% = 1
                End If

                If s = "Shipto" Then
                    If sv% = 0 Then
                        sr = md(b2, 191, 10) : custnum1 = Val(sr)
                        SAVACK(cni, cni2, inv, custnum1, custcd1)
                        sv% = 1
                    End If
                    sr = md(b2, 7, 30)
                    nm = Trim(sr)
                    sr = md(b2, 37, 30)
                    addr = Trim(sr)
                    sr = md(b2, 67, 30)
                    addr2 = Trim(sr)
                    sr = md(b2, 97, 20)
                    city = Trim(sr)
                    sr = md(b2, 117, 2)
                    sstate = sr
                    sr = md(b2, 119, 10)
                    zip = Trim(sr)
                    fs2.WriteLine("N1*ST*" & nm & "*67*" & refer & i6) : seg = seg + 1
                    fs2.WriteLine("N3*" & addr & "*" & addr2 & i6) : seg = seg + 1
                    fs2.WriteLine("N4*" & city & "*" & sstate & "*" & zip & i6)
                    seg = seg + 1
                    fs2.WriteLine("DTM*011*" & dt8 & i6) : seg = seg + 1
                End If
                If s = "Item" Then
                    sr = md(b2, 11, 10)
                    qty = CStr(Val(sr))
                    sr = md(b2, 23, 14)
                    pr = Val(sr)
                    sr = md(b2, 63, 24)
                    it = Trim(sr)
                    sr = md(b2, 87, 24)
                    desc = Trim(sr)
                    iti = iti + 1
                    fs2.WriteLine("IT1*" & Format(iti, "00000") & "*" & qty & "*EA*" &
                        Trim(Format(pr, "#####0.00#")) & "**VN*" & it & i6)
                    seg = seg + 1
                    ht = ht + Val(qty)

                    sql1 = "select icsp.cono, icsp.prod, icsp.pbseqno FROM pub.icsp " &
                          "WHERE icsp.cono = 1 AND icsp.prod = '" & it & "'"

                    rs2.Open(sql1, cn2, 3, 1, 1)

                    If Not rs2.EOF Then

                        If Trim(rs2.Fields(2).Value) <> "" Then
                            fs2.WriteLine("PID*X*MAC*UN*" & rs2.Fields(2).Value & "*14.0801**SPSC")
                            seg = seg + 1
                        End If
                    End If
                    If rs2.State = 1 Then
                        rs2.Close()
                    End If

                    If Trim(desc) = "" Then desc = it
                    fs2.WriteLine("PID*F****" & desc & i6)
                    seg = seg + 1
                End If
                If s = "Addon" Then
                    sr = md(b2, 8, 12)
                    frt = frt + Val(sr)
                End If

                If s = "Total" Then
                    sr = md(b2, 13, 14)
                    tot = Val(sr)
                    fs2.WriteLine("TDS*" & Trim(Format(tot * 100, "##########")) & i6)
                    seg = seg + 1
                    strtot = CStr(Format(tot, "###,##0.00"))
                    po = Mid(po, 1, 20)

                    sndrpt = sndrpt & cusn & Space(10 - Len(cusn)) & po & Space(25 - Len(po)) & inv & Space(14 - Len(inv) + 11 - Len(strtot)) &
                          Format(tot, "####,##0.00") & vbNewLine & wrko & "  " & jobn & vbNewLine
                    'fshist.WriteLine(sndrpt)
                    If ttax <> 0 Then
                        fs2.WriteLine("TXI*TX*" & Trim(Format(ttax, "#####0.00")) & i6)
                        seg = seg + 1
                    End If
                    If shipd = "" Then shipd = "FEDEX/RPS GROUND"
                    fs2.WriteLine("CAD*T****" & shipd & i6)
                    seg = seg + 1
                    If frt <> 0 Then
                        fs2.WriteLine("SAC*C*D240***" & Trim(Format(frt * 100, "########")) &
                            "*******06***FREIGHT" & i6)
                        seg = seg + 1
                    End If

                    fs2.WriteLine("CTT*" & Trim(Str(iti)) & i6)
                    seg = seg + 1
                    fs2.WriteLine("SE*" & Trim(Str(seg + 1)) & "*" & Format(cni2, "000000000") & i6)

                    seg = 0
                End If
            Loop
            If cni2 > 0 Then
                fs2.WriteLine("GE*" & CStr(Format(cni2, "########0")) &
                     "*" & CStr(Format(cni, "########0")) & i6)

                fs2.WriteLine("IEA*" & CStr(Format(1, "########0")) &
                     "*" & CStr(Format(cni, "000000000")) & i6)
                cni2 = 0
            End If
            fs1.close()
            wf = Dir()
        End While
        fs2.Close()
        fshist.WriteLine(sndrpt)
        fshist.Close()
BGO:

        Dim errh As Boolean = False
        wf = Dir("c:\edi\sxout\cust*.*")
        While String.IsNullOrEmpty(wf) = False
            fnamep = "c:\edi\sxout\" & wf
            fso = New FileInfo(fnamep)
            If fso.Length > 0 Then

                '   putf2(fnamep, errh)

                FtpUploadFile(fnamep, wf, errh)
                If errh = False Then
                    File.Move(fnamep, "c:\edi\histout\" & wf)
                End If
            Else : File.Delete(fnamep)
            End If
            wf = Dir()
        End While


        wf = Dir("e:\apps\cust\e*.*")
        fx% = 0
        While String.IsNullOrEmpty(wf) = False
WF3:
            fx% = fx% + 1
            fnamep = "cust" & CStr(Format(Now, "yyMMdd")) & CStr(Format(Now, "hhmm")) & CStr(fx%) & ".txt"

            If File.Exists("c:\edi\history\" & fnamep) Then GoTo WF3
            File.Move("e:\apps\cust\" & wf, "c:\edi\History\" & fnamep)

            wf = Dir()
        End While
CUST599:
    End Sub
    Sub LGEN(ByRef cni As Long, ByRef cni2 As Long, ByRef lx1%, ByVal sndrpt As String)

        Dim cn2 As ADODB.Connection
        Dim rs2 As ADODB.Recordset
        rs2 = New ADODB.Recordset
        cn2 = New ADODB.Connection
        cn2 = CreateObject("ADODB.Connection")
        cn2.Open("con", "user", "p")

        Dim dt1 As Date = Now
        Dim wf As String
        Dim ck% = 0
        Dim i6 As String = ""
        Dim seg As Long = 0
        Dim b As String
        Dim b2 As String
        Dim dt8 As String = ""
        Dim s As String
        Dim sr As String
        Dim po As String = ""
        Dim inv As String = ""
        Dim qty As Long
        Dim pr As Double
        Dim fx%
        Dim it As String
        Dim desc As String
        Dim po1 As String = ""
        Dim po2 As String = ""
        Dim task As String
        Dim wrko As String = ""
        Dim jobn As String = ""
        Dim shipd As String = ""
        Dim refer As String = ""
        Dim invdt8 As String
        Dim invdt As String
        Dim custnum1 As Long
        Dim nm As String
        Dim sql1 As String
        Dim addr As String
        Dim addr2 As String
        Dim city As String
        Dim sstate As String
        Dim tot As Double
        Dim zip As String
        Dim custcd1 As String = "CUST_DL"
        Dim cusn As String
        cusn = "CUST"
        Dim strtot As String

        Dim sv%
        Dim p7%
        Dim p8%
        Dim p9%
        Dim ttax As Double
        Dim frt As Double
        Dim iti As Long
        Dim ht As Long
        Dim fso As FileInfo
        Dim fsz% = 0

        Dim fshist As New StreamWriter("c:\edi\cust_dl.txt", True)

        wf = Dir("e:\apps\cust\e*.*")
        While String.IsNullOrEmpty(wf) = False

            ' Check for data to process.  If not then end.
            fso = New FileInfo("e:\apps\cust\" & wf)
            If fso.Length > 0 Then fsz% = fsz% + 1 Else File.Delete("e:\apps\cust\" & wf)

            wf = Dir()
        End While
        If fsz% = 0 Then fshist.Close() : GoTo BGO

        Dim fnamep1 As String = "ejl" & Format(Now, "yyMMdd") & Format(Now, "hhmmff") & ".txt"
        Dim fnamep As String = "c:\edi\sxout\" & fnamep1
        Dim fs1
        Dim fs2 As New StreamWriter(fnamep, False)
        wf = Dir("e:\apps\cust\*.*")

        While String.IsNullOrEmpty(wf) = False
            fs1 = New StreamReader("e:\apps\cust\" & wf)

            Do While fs1.peek >= 0
RR4:
                b = fs1.readline


                b2 = CStr(b)
                s = Trim(Left(b, 6).ToString)

                If s = "Inv" Then

                    sr = md(b2, 24, 22)
                    po = Trim(sr)
                    sr = md(b2, 619, 8)
                    inv = sr
                    inv = inv & "-" & md(b2, 22, 2)

                    b = b2
                    sv% = 0
                    iti = 0 : ht = 0 : frt = 0

                    sr = md(b2, 7, 8)
                    dt1 = Date.Parse(sr)
                    invdt8 = Format(dt1, "yyyyMMdd")
                    invdt = Format(dt1, "yyMMdd")

                    p9% = InStr(1, po, "V#")
                    If p9% > 0 Then
                        p8% = InStr(1, po, " ")
                        If p8% > 0 Then
                            p7% = Len(po)
                            po1 = Mid(po, 1, p8% - 1)
                            po2 = Mid(po, p8% + 1, p7% - p8%)
                        End If
                    Else
                        po1 = po : po2 = ""
                    End If

                    sr = md(b2, 58, 3)
                    refer = Trim(sr)
                    If refer = "EDI" Then refer = ""
                    sr = md(b2, 61, 21)
                    task = Trim(sr)
                    sr = md(b2, 592, 10)
                    wrko = Trim(sr)
                    sr = md(b2, 562, 30)
                    jobn = Trim(sr)
                    sr = md(b2, 147, 13)
                    ttax = Val(sr)
                    sr = md(b2, 306, 24)
                    shipd = Trim(sr)
                    ISAJCL(cni2, cni, seg, po2, invdt8, fs2, inv, po1, dt8)
                    lx1% = lx1% + 1
                    'GoSub SAVACK
                    If task <> "" Then
                        fs2.WriteLine("REF*K5*" & task & i6) : seg = seg + 1
                    End If

                    If wrko <> "" Then
                        fs2.WriteLine("REF*WO*" & wrko & i6) : seg = seg + 1
                    End If
                    fs2.WriteLine("PER*BI*" & jobn & i6) : seg = seg + 1
                    fs2.WriteLine("PER*OD*" & refer & i6) : seg = seg + 1
                End If
                If s = "Custom" Then
                    sr = md(b2, 165, 10) : custnum1 = Val(sr)
                    SAVACK(cni, cni2, inv, custnum1, custcd1)
                    sv% = 1
                End If

                If s = "Shipto" Then
                    If sv% = 0 Then
                        sr = md(b2, 191, 10) : custnum1 = Val(sr)
                        SAVACK(cni, cni2, inv, custnum1, custcd1)
                        sv% = 1
                    End If
                    sr = md(b2, 7, 30)
                    nm = Trim(sr)
                    sr = md(b2, 37, 30)
                    addr = Trim(sr)
                    sr = md(b2, 67, 30)
                    addr2 = Trim(sr)
                    sr = md(b2, 97, 20)
                    city = Trim(sr)
                    sr = md(b2, 117, 2)
                    sstate = sr
                    sr = md(b2, 119, 10)
                    zip = Trim(sr)
                    fs2.WriteLine("N1*ST*" & nm & "*67*" & refer & i6) : seg = seg + 1
                    fs2.WriteLine("N3*" & addr & "*" & addr2 & i6) : seg = seg + 1
                    fs2.WriteLine("N4*" & city & "*" & sstate & "*" & zip & i6)
                    seg = seg + 1
                    fs2.WriteLine("DTM*011*" & dt8 & i6) : seg = seg + 1
                End If
                If s = "Item" Then
                    sr = md(b2, 11, 10)
                    qty = CStr(Val(sr))
                    sr = md(b2, 23, 14)
                    pr = Val(sr)
                    sr = md(b2, 63, 24)
                    it = Trim(sr)
                    sr = md(b2, 87, 24)
                    desc = Trim(sr)
                    iti = iti + 1
                    fs2.WriteLine("IT1*" & Format(iti, "00000") & "*" & qty & "*EA*" &
                        Trim(Format(pr, "#####0.00#")) & "**VN*" & it & i6)
                    seg = seg + 1
                    ht = ht + Val(qty)

                    sql1 = "select icsp.cono, icsp.prod, icsp.pbseqno FROM pub.icsp " &
                          "WHERE icsp.cono = 1 AND icsp.prod = '" & it & "'"

                    rs2.Open(sql1, cn2, 3, 1, 1)

                    If Not rs2.EOF Then

                        If Trim(rs2.Fields(2).Value) <> "" Then
                            fs2.WriteLine("PID*X*MAC*UN*" & rs2.Fields(2).Value & "*14.0801**SPSC")
                            seg = seg + 1
                        End If
                    End If
                    If rs2.State = 1 Then
                        rs2.Close()
                    End If

                    If Trim(desc) = "" Then desc = it
                    fs2.WriteLine("PID*F****" & desc & i6)
                    seg = seg + 1
                End If
                If s = "Addon" Then
                    sr = md(b2, 8, 12)
                    frt = frt + Val(sr)
                End If

                If s = "Total" Then
                    sr = md(b2, 13, 14)
                    tot = Val(sr)
                    fs2.WriteLine("TDS*" & Trim(Format(tot * 100, "##########")) & i6)
                    seg = seg + 1
                    strtot = CStr(Format(tot, "###,##0.00"))
                    po = Mid(po, 1, 20)

                    sndrpt = sndrpt & cusn & Space(10 - Len(cusn)) & po & Space(25 - Len(po)) & inv & Space(14 - Len(inv) + 11 - Len(strtot)) &
                          Format(tot, "####,##0.00") & vbNewLine & wrko & "  " & jobn & vbNewLine
                    If ttax <> 0 Then
                        fs2.WriteLine("TXI*TX*" & Trim(Format(ttax, "#####0.00")) & i6)
                        seg = seg + 1
                    End If
                    If shipd = "" Then shipd = "FEDEX/RPS GROUND"
                    fs2.WriteLine("CAD*T****" & shipd & i6)
                    seg = seg + 1
                    If frt <> 0 Then
                        fs2.WriteLine("SAC*C*D240***" & Trim(Format(frt * 100, "########")) &
                            "*******06***FREIGHT" & i6)
                        seg = seg + 1
                    End If

                    fs2.WriteLine("CTT*" & Trim(Str(iti)) & i6)
                    seg = seg + 1
                    fs2.WriteLine("SE*" & Trim(Str(seg + 1)) & "*" & Format(cni2, "000000000") & i6)

                    seg = 0
                End If
            Loop
            If cni2 > 0 Then
                fs2.WriteLine("GE*" & CStr(Format(cni2, "########0")) &
                     "*" & CStr(Format(cni, "########0")) & i6)

                fs2.WriteLine("IEA*" & CStr(Format(1, "########0")) &
                     "*" & CStr(Format(cni, "000000000")) & i6)
                cni2 = 0
            End If
            fs1.close()
            wf = Dir()
        End While
        fs2.Close()
        fshist.WriteLine(sndrpt)
        fshist.Close()
BGO:

        Dim errh As Boolean = False
        wf = Dir("c:\edi\sxout\cust*.*")
        While String.IsNullOrEmpty(wf) = False
            fnamep = "c:\edi\sxout\" & wf
            fso = New FileInfo(fnamep)
            If fso.Length > 0 Then

                FtpUploadFile(fnamep, wf, errh)

                If errh = False Then
                    File.Move(fnamep, "c:\edi\histout\" & wf)
                End If
            Else : File.Delete(fnamep)
            End If
            wf = Dir()
        End While


        wf = Dir("e:\apps\cust\e*.*")
        fx% = 0
        While String.IsNullOrEmpty(wf) = False
WF3:
            fx% = fx% + 1
            fnamep = "custl" & CStr(Format(Now, "yyMMdd")) & CStr(Format(Now, "hhmm")) & CStr(fx%) & ".txt"

            If File.Exists("c:\edi\history\" & fnamep) Then GoTo WF3
            File.Move("e:\apps\cust\" & wf, "c:\edi\History\" & fnamep)

            wf = Dir()
        End While
CUST599:
    End Sub
    Sub CUSTScreen(ByRef fr1 As StreamReader, ByRef bb As String, ByRef ck%)
        Dim po As String
        ck% = 0
        po = Trim(Mid(bb, 24, 22))
        If Mid(po, 1, 2) = "US" Then GoTo JJ72
        If Mid(bb, 641, 6) = "NUMBER" Then GoTo JJ72
        If IsNumeric(po) = False Then GoTo JJ89
JJ72:
        Dim cn As ADODB.Connection
        Dim rs As ADODB.Recordset
        cn = New ADODB.Connection
        rs = New ADODB.Recordset
        cn = CreateObject("ADODB.Connection")
        cn.Open("con", "user", "p")
        Dim sql As String
        Dim ord As Long
        ord = Val(Mid(bb, 619, 8))

        sql = "Select oeeh.orderno, oeeh.cono From pub.oeeh " &
            "WHERE oeeh.orderno = " & ord & " and oeeh.cono = " & 1 & ""
        rs.Open(sql, cn, 3, 1, 1)
        If rs.RecordCount > 1 Then
            mannotice()
            While fr1.Peek >= 0
                bb = fr1.ReadLine
                If Mid(bb, 1, 5) = "Total" Then ck% = 1 : Exit While
            End While
        End If

        If rs.State = 1 Then rs.Close()
        cn.Close()
        rs = Nothing
        cn = Nothing
JJ89:


    End Sub
    Sub mannotice()
        Dim em As appsend.Appsend = New appsend.Appsend
        em.Sendm("email@email.com", "", "", "", "", "Manual EDI Flag", " ")
        em = Nothing

    End Sub


    Sub FtpUploadFile(ByVal filetoupload As String, fnm As String, ByRef err As Boolean)
        ' Create a web request that will be used to talk with the server and set the request method to upload a file by ftp.
        Dim ftpRequest As FtpWebRequest = CType(WebRequest.Create("ftp://url.com/" & fnm), FtpWebRequest)

        Try
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile

            ' Confirm the Network credentials based on the user name and password passed in.
            ftpRequest.Credentials = New NetworkCredential("cred1", "cred2")

            ' Read into a Byte array the contents of the file to be uploaded 
            Dim bytes() As Byte = System.IO.File.ReadAllBytes(filetoupload)

            ' Transfer the byte array contents into the request stream, write and then close when done.
            ftpRequest.ContentLength = bytes.Length
            Using UploadStream As Stream = ftpRequest.GetRequestStream()
                UploadStream.Write(bytes, 0, bytes.Length)
                UploadStream.Close()
            End Using
        Catch ex As Exception

            err = True
            Exit Sub
        End Try
        err = False

    End Sub
    Private Function md(b1 As String, i1 As Single, i2 As Single) As String
        md = Mid(b1, i1, i2)

    End Function
    Public Function Appstart() As String
        Dim fs2 = New StreamReader("c:\edata\hold\check2.txt")
        Appstart = fs2.ReadLine
        fs2.Close()
        Dim fs3 = New StreamWriter("c:\edata\hold\check2.txt", False)
        fs3.WriteLine("1")
        fs3.Close()
    End Function
    Sub Append(ByVal cnoo As String)
        Dim fs3 = New StreamWriter("c:\edata\hold\check2.txt", False)
        fs3.WriteLine(cnoo)
        fs3.Close()
    End Sub
    Public Function Constart() As Long
        Dim fs2 = New StreamReader("c:\edata\hold\econtrol.txt")
        Constart = fs2.ReadLine
        fs2.Close()
    End Function
    Sub Conend(ByVal cnoo As Long)
        Dim fs3 = New StreamWriter("c:\edata\hold\econtrol.txt", False)
        fs3.WriteLine(cnoo)
        fs3.Close()
    End Sub
End Module
