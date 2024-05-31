Imports System.Net.Security
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Oracle.ManagedDataAccess.Client
Imports System.Data.SqlClient
Imports System.Windows.Forms.VisualStyles.VisualStyleElement


Public Class Form1


    Private Sub textBox2_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cuentabox.KeyPress
        If InStr(1, "0123456789, -" & Chr(8), e.KeyChar) = 0 Then
            e.KeyChar = ""
        End If
    End Sub

    Private Sub Form1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles MyBase.KeyPress
        If Char.IsLetter(e.KeyChar) Then
            e.Handled = False
        ElseIf Char.IsControl(e.KeyChar) Then
            e.Handled = False
        ElseIf Char.IsSeparator(e.KeyChar) Then
            e.Handled = False
        Else
            e.Handled = True
        End If
    End Sub



    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        MostrarTransacciones()
    End Sub

    ' Método que ejecuta el procedimiento almacenado y llena el DataGridView
    Private Sub MostrarTransacciones()
        ' Cadena de conexión a la base de datos Oracle
        Dim connectionString As String = My.Settings.conexion

        ' Crear la conexión usando la cadena de conexión
        Using conn As New OracleConnection(connectionString)
            ' Crear el comando para ejecutar el procedimiento almacenado
            Using cmd As New OracleCommand("ver_transacciones", conn)
                cmd.CommandType = CommandType.StoredProcedure

                ' Crear y configurar el parámetro de entrada (número de cuenta)
                Dim paramNoCuenta As New OracleParameter("p_no_cuenta", OracleDbType.Int32)
                paramNoCuenta.Value = Convert.ToInt32(cuentabox.Text) ' Obtener el número de cuenta del TextBox
                cmd.Parameters.Add(paramNoCuenta)

                ' Crear el parámetro de salida (cursor)
                Dim refCursor As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                refCursor.Direction = ParameterDirection.Output
                cmd.Parameters.Add(refCursor)

                Try
                    ' Abrir la conexión
                    conn.Open()

                    ' Crear el adaptador para llenar el DataTable con los datos del cursor
                    Using da As New OracleDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        DataGridView1.DataSource = dt
                    End Using

                    MessageBox.Show("Datos obtenidos exitosamente.")
                Catch ex As Exception
                    MessageBox.Show("Error: " & ex.Message)
                Finally
                    ' Cerrar la conexión
                    conn.Close()
                End Try
            End Using
        End Using
    End Sub


    'limpiar campos de salida
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        cuentabox.Clear()
        TextBox5.Clear()
        textBox4.Clear()
        TextBox2.Clear()
        textBox1.Clear()
        textBox3.Clear()
        DataGridView1.Controls.Clear()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim IDcuenta As String = cuentabox.Text.Trim()

        If IDcuenta <> "" Then
            Using conn As New OracleConnection(My.Settings.conexion)
                Try
                    conn.Open()

                    ' Llamar al procedimiento almacenado
                    Using cmd As New OracleCommand("ver_cuentas", conn)
                        cmd.CommandType = CommandType.StoredProcedure

                        ' Parámetro de entrada
                        cmd.Parameters.Add("p_no_cuenta", OracleDbType.Int32).Value = Convert.ToInt32(IDcuenta)

                        ' Parámetro de salida
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output

                        ' Ejecutar el comando
                        Using reader As OracleDataReader = cmd.ExecuteReader()
                            If reader.HasRows Then
                                While reader.Read()
                                    ' Asignar los valores a los TextBoxes
                                    textBox1.Text = reader("balance_total").ToString()
                                    textBox4.Text = reader("balance_disponible").ToString()
                                    TextBox5.Text = reader("nombre_cliente").ToString()
                                    TextBox2.Text = reader("DPI_cliente").ToString()
                                End While
                            Else
                                MessageBox.Show("No se encontró un cliente con ese número de cuenta.")
                            End If
                        End Using
                    End Using

                Catch ex As Exception
                    MessageBox.Show("Error al buscar el cliente: " & ex.Message)
                End Try
            End Using
        Else
            MessageBox.Show("Por favor, ingrese un número de cuenta válido.")
        End If

    End Sub

    Private Sub textBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles textBox1.KeyPress
        e.Handled = True
        MsgBox("Este campo no permite la entrada de datos.")
    End Sub

    Private Sub textBox4_KeyPress(sender As Object, e As KeyPressEventArgs) Handles textBox4.KeyPress
        e.Handled = True
        MsgBox("Este campo no permite la entrada de datos.")
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

    End Sub
End Class
