import { useEffect, useState } from 'react';
import { api } from './api';
import { LABEL, STATUS } from './config';

const fmt = (k, v) => {
  if (v == null || v === '') return '—';
  if (k === 'status') {
    const cor = { 1: 'success', 2: 'warning text-dark', 3: 'secondary' }[v];
    return <span className={`badge bg-${cor}`}>{STATUS[v]}</span>;
  }
  if (k.startsWith('data')) return new Date(v).toLocaleDateString('pt-BR');
  if (k.startsWith('valor')) return Number(v).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  return String(v);
};

// Carrega as opções de um select (rota da API ou lista fixa)
function useOpcoes(campos) {
  const [opcoes, setOpcoes] = useState({});
  useEffect(() => {
    campos.forEach(async c => {
      if (c.opts) return setOpcoes(o => ({ ...o, [c.n]: c.opts }));
      if (!c.from) return;
      const dados = await api(c.from).catch(() => []);
      setOpcoes(o => ({ ...o, [c.n]: dados.map(r => ({ v: r.id, t: c.lbl(r) })) }));
    });
  }, []);
  return opcoes;
}

function Campo({ c, valor, onChange, opcoes }) {
  const comum = { className: 'form-control', required: !!c.req, value: valor ?? '', onChange: e => onChange(e.target.value) };
  if (c.t === 'select')
    return (
      <select {...comum} className="form-select">
        <option value="">Selecione…</option>
        {(opcoes || []).map(o => <option key={o.v} value={o.v}>{o.t}</option>)}
      </select>
    );
  return <input {...comum} type={c.t || 'text'} step={c.step} maxLength={c.max} />;
}

function Modal({ titulo, campos, inicial, onSubmit, onClose }) {
  const [v, setV] = useState(inicial);
  const [erro, setErro] = useState('');
  const opcoes = useOpcoes(campos);

  const salvar = async e => {
    e.preventDefault();
    const body = { ...v };
    campos.forEach(c => {
      if (c.t === 'number' || c.t === 'select') body[c.n] = body[c.n] === '' || body[c.n] == null ? null : Number(body[c.n]);
    });
    try { await onSubmit(body); } catch (x) { setErro(x.message); }
  };

  return (
    <div className="modal d-block" style={{ background: '#0006' }}>
      <div className="modal-dialog">
        <form className="modal-content" onSubmit={salvar}>
          <div className="modal-header"><h5 className="modal-title">{titulo}</h5></div>
          <div className="modal-body">
            {erro && <div className="alert alert-danger py-2">{erro}</div>}
            {campos.map(c => (
              <div className="mb-3" key={c.n}>
                <label className="form-label small fw-semibold">{c.l}</label>
                <Campo c={c} valor={v[c.n]} opcoes={opcoes[c.n]} onChange={x => setV({ ...v, [c.n]: x })} />
              </div>
            ))}
          </div>
          <div className="modal-footer">
            <button type="button" className="btn btn-light" onClick={onClose}>Cancelar</button>
            <button className="btn btn-primary">Salvar</button>
          </div>
        </form>
      </div>
    </div>
  );
}

// Consultas que usam as rotas de filtro do back-end (JOINs do TP1)
function Consultas({ queries, onRun, onClear }) {
  const [i, setI] = useState(0);
  const [p, setP] = useState({});
  const q = queries[i];
  const opcoes = useOpcoes(q.params);

  return (
    <form className="card card-body mb-3" onSubmit={e => { e.preventDefault(); onRun({ q, p }); }}>
      <div className="row g-2 align-items-end">
        <div className="col-md-4">
          <label className="form-label small fw-semibold">Consulta no banco</label>
          <select className="form-select" value={i} onChange={e => { setI(+e.target.value); setP({}); }}>
            {queries.map((x, k) => <option key={k} value={k}>{x.label}</option>)}
          </select>
        </div>
        {q.params.map(c => (
          <div className="col-md-3" key={c.n}>
            <label className="form-label small fw-semibold">{c.l}</label>
            <Campo c={{ ...c, t: c.t || 'select' }} valor={p[c.n]} opcoes={opcoes[c.n]} onChange={x => setP({ ...p, [c.n]: x })} />
          </div>
        ))}
        <div className="col-auto">
          <button className="btn btn-primary me-2">Consultar</button>
          <button type="button" className="btn btn-outline-secondary" onClick={onClear}>Limpar</button>
        </div>
      </div>
    </form>
  );
}

export default function Crud({ cfg }) {
  const [linhas, setLinhas] = useState([]);
  const [erro, setErro] = useState('');
  const [filtro, setFiltro] = useState({});
  const [view, setView] = useState(null);
  const [modal, setModal] = useState(null);
  const opcoesFiltro = useOpcoes((cfg.filters || []).filter(f => f.opts));

  const carregar = async v => {
    try {
      setErro('');
      setLinhas(await api(v ? v.q.url(v.p) : cfg.path));
      setView(v);
    } catch (e) { setErro(e.message); setLinhas([]); }
  };
  useEffect(() => { carregar(null); }, []);

  const excluir = async r => {
    if (!confirm(`Excluir o registro #${r.id}?`)) return;
    try { await api(`${cfg.path}/${r.id}`, 'DELETE'); carregar(view); } catch (e) { setErro(e.message); }
  };

  const abrir = (mode, r) => {
    const campos = cfg.fields.filter(f => !f.only || f.only === mode);
    setModal({ titulo: `${mode === 'edit' ? 'Editar' : 'Novo'} — ${cfg.title}`, campos, inicial: r || {},
      submit: b => (mode === 'edit' ? api(`${cfg.path}/${r.id}`, 'PUT', b) : api(cfg.path, 'POST', b)) });
  };
  const abrirAcao = (a, r) =>
    setModal({ titulo: `${a.l} — #${r.id}`, campos: a.fields, inicial: {},
      submit: b => { const x = a.run(r, b); return api(x.path, x.method, x.body); } });

  const mostradas = linhas.filter(r => (cfg.filters || []).every(f => {
    const v = filtro[f.n];
    if (!v) return true;
    const cel = String(r[f.n] ?? '');
    return f.opts ? cel === v : cel.toLowerCase().includes(v.toLowerCase());
  }));

  return (
    <>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h3 className="m-0">{cfg.title}</h3>
        <button className="btn btn-primary" onClick={() => abrir('new')}>+ Novo</button>
      </div>

      {cfg.queries && <Consultas queries={cfg.queries} onRun={carregar} onClear={() => carregar(null)} />}

      <div className="card card-body mb-3">
        <div className="row g-2">
          {(cfg.filters || []).map(f => (
            <div className="col-md-3" key={f.n}>
              {f.opts ? (
                <select className="form-select" value={filtro[f.n] || ''} onChange={e => setFiltro({ ...filtro, [f.n]: e.target.value })}>
                  <option value="">{f.l}: todos</option>
                  {(opcoesFiltro[f.n] || []).map(o => <option key={o.v} value={o.v}>{o.t}</option>)}
                </select>
              ) : (
                <input className="form-control" placeholder={`Filtrar por ${f.l.toLowerCase()}`} value={filtro[f.n] || ''}
                  onChange={e => setFiltro({ ...filtro, [f.n]: e.target.value })} />
              )}
            </div>
          ))}
        </div>
      </div>

      {erro && <div className="alert alert-danger">{erro}</div>}

      <div className="card">
        <div className="table-responsive">
          <table className="table table-hover align-middle m-0">
            <thead><tr>{cfg.cols.map(c => <th key={c}>{LABEL[c]}</th>)}<th /></tr></thead>
            <tbody>
              {mostradas.map(r => (
                <tr key={r.id}>
                  {cfg.cols.map(c => <td key={c}>{fmt(c, r[c])}</td>)}
                  <td className="text-end text-nowrap">
                    {(cfg.actions || []).filter(a => a.show(r)).map(a => (
                      <button key={a.l} className="btn btn-sm btn-success me-1" onClick={() => abrirAcao(a, r)}>{a.l}</button>
                    ))}
                    {!cfg.noEdit && (!cfg.canEdit || cfg.canEdit(r)) && <button className="btn btn-sm btn-outline-primary me-1" onClick={() => abrir('edit', r)}>Editar</button>}
                    <button className="btn btn-sm btn-outline-danger" onClick={() => excluir(r)}>Excluir</button>
                  </td>
                </tr>
              ))}
              {!mostradas.length && <tr><td colSpan={cfg.cols.length + 1} className="text-center text-muted py-4">Nenhum registro encontrado.</td></tr>}
            </tbody>
          </table>
        </div>
      </div>

      {modal && (
        <Modal {...modal} onClose={() => setModal(null)}
          onSubmit={async b => { await modal.submit(b); setModal(null); carregar(view); }} />
      )}
    </>
  );
}