using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Gerador.Util;

namespace Gerador
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            if (this.txtUsuario.Text == String.Empty)
            {
                MessageBox.Show("Usuário está vazio", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                this.txtUsuario.Focus();
                this.txtUsuario.Select();
                return ;
            }

            if (this.txtSenha.Text == String.Empty)
            {
                MessageBox.Show("Senha está vazio", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                this.txtSenha.Focus();
                this.txtSenha.Select();
                return;
            }

            if (this.txtBanco.Text == String.Empty)
            {
                MessageBox.Show("Banco está vazio", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                this.txtBanco.Focus();
                this.txtBanco.Select();
                return;
            }

            Conexao.Instance.Usuario    = this.txtUsuario.Text;
            Conexao.Instance.Senha = this.txtSenha.Text;
            Conexao.Instance.Banco = this.txtBanco.Text;

            Conexao.Instance.Open();

            this.DialogResult = DialogResult.OK;
        }
    }
}