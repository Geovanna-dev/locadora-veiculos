import { useEffect, useState } from 'react';
import { api } from './api';
import { ENTIDADES } from './config';
import Crud from './Crud';

function Home() {
  const [d, setD] = useState(null);
  const [erro, setErro] = useState('');
  useEffect(() => {
    Promise.all(['veiculos', 'veiculos/disponiveis', 'clientes', 'alugueis/em-aberto'].map(p => api(p)))
      .then(([v, disp, c, ab]) => setD({ v, disp, c, ab }))
      .catch(e => setErro(`Não foi possível falar com a API: ${e.message}`));
  }, []);

  if (erro) return <div className="alert alert-danger">{erro}</div>;
  if (!d) return <p>Carregando…</p>;
  const cards = [['Veículos na frota', d.v.length], ['Disponíveis agora', d.disp.length],
    ['Clientes', d.c.length], ['Aluguéis em aberto', d.ab.length]];
  return (
    <>
      <h3 className="mb-3">Painel</h3>
      <div className="row g-3 mb-4">
        {cards.map(([t, n]) => (
          <div className="col-md-3" key={t}><div className="card card-body"><div className="stat">{n}</div><div className="text-muted">{t}</div></div></div>
        ))}
      </div>
      <div className="card">
        <div className="card-header bg-white fw-semibold">Devoluções pendentes (mais próximas primeiro)</div>
        <table className="table m-0">
          <tbody>
            {d.ab.slice(0, 8).map(a => (
              <tr key={a.id}><td>{a.clienteNome}</td><td>{a.veiculoModelo} ({a.veiculoPlaca})</td>
                <td>{new Date(a.dataPrevistaDevolucao).toLocaleDateString('pt-BR')}</td></tr>
            ))}
            {!d.ab.length && <tr><td className="text-muted">Nenhum aluguel em aberto.</td></tr>}
          </tbody>
        </table>
      </div>
    </>
  );
}

export default function App() {
  const [pagina, setPagina] = useState('home');
  const abas = [['home', 'Painel'], ...Object.entries(ENTIDADES).map(([k, e]) => [k, e.title])];
  return (
    <>
      <nav className="topbar navbar navbar-expand mb-4">
        <div className="container">
          <span className="brand navbar-brand">🔑 Locadora</span>
          <ul className="navbar-nav">
            {abas.map(([k, t]) => (
              <li className="nav-item" key={k}>
                <a className={`nav-link ${pagina === k ? 'active' : ''}`} onClick={() => setPagina(k)}>{t}</a>
              </li>
            ))}
          </ul>
        </div>
      </nav>
      <main className="container pb-5">
        {pagina === 'home' ? <Home /> : <Crud key={pagina} cfg={ENTIDADES[pagina]} />}
      </main>
    </>
  );
}
