// Descreve cada entidade do TP1: rotas, campos do formulário, colunas, filtros e consultas.
export const STATUS = { 1: 'Disponível', 2: 'Alugado', 3: 'Manutenção' };
const statusOpts = Object.entries(STATUS).map(([v, t]) => ({ v, t }));

export const LABEL = {
  id: '#', nome: 'Nome', paisOrigem: 'País', quantidadeVeiculos: 'Veículos', descricao: 'Descrição',
  valorDiariaBase: 'Diária base', cpf: 'CPF', email: 'E-mail', telefone: 'Telefone', modelo: 'Modelo',
  anoFabricacao: 'Ano', quilometragem: 'Km', placa: 'Placa', cor: 'Cor', status: 'Status',
  fabricanteNome: 'Fabricante', categoriaNome: 'Categoria', clienteNome: 'Cliente',
  veiculoModelo: 'Veículo', veiculoPlaca: 'Placa', dataRetirada: 'Retirada',
  dataPrevistaDevolucao: 'Prev. devolução', dataDevolucao: 'Devolução', valorTotal: 'Total',
};

export const ENTIDADES = {
  veiculos: {
    title: 'Veículos', path: 'veiculos',
    fields: [
      { n: 'modelo', l: 'Modelo', req: 1 },
      { n: 'anoFabricacao', l: 'Ano de fabricação', t: 'number', req: 1 },
      { n: 'quilometragem', l: 'Quilometragem', t: 'number' },
      { n: 'placa', l: 'Placa (7 caracteres, sem hífen)', req: 1, max: 7 },
      { n: 'cor', l: 'Cor' },
      { n: 'fabricanteId', l: 'Fabricante', t: 'select', from: 'fabricantes', lbl: r => r.nome, req: 1 },
      { n: 'categoriaId', l: 'Categoria', t: 'select', from: 'categorias', lbl: r => r.nome, req: 1 },
      { n: 'status', l: 'Status', t: 'select', opts: statusOpts, only: 'edit', req: 1 },
    ],
    cols: ['id', 'modelo', 'placa', 'fabricanteNome', 'categoriaNome', 'anoFabricacao', 'quilometragem', 'status'],
    filters: [
      { n: 'modelo', l: 'Modelo' }, { n: 'placa', l: 'Placa' },
      { n: 'fabricanteNome', l: 'Fabricante' }, { n: 'status', l: 'Status', opts: statusOpts },
    ],
    queries: [
      { label: 'Veículos disponíveis (por categoria)',
        params: [{ n: 'categoriaId', l: 'Categoria (opcional)', from: 'categorias', lbl: r => r.nome }],
        url: p => `veiculos/disponiveis${p.categoriaId ? `?categoriaId=${p.categoriaId}` : ''}` },
      { label: 'Veículos por fabricante',
        params: [{ n: 'fabricanteId', l: 'Fabricante', from: 'fabricantes', lbl: r => r.nome, req: 1 }],
        url: p => `veiculos/por-fabricante/${p.fabricanteId}` },
    ],
  },
  fabricantes: {
    title: 'Fabricantes', path: 'fabricantes',
    fields: [{ n: 'nome', l: 'Nome', req: 1 }, { n: 'paisOrigem', l: 'País de origem' }],
    cols: ['id', 'nome', 'paisOrigem', 'quantidadeVeiculos'],
    filters: [{ n: 'nome', l: 'Nome' }, { n: 'paisOrigem', l: 'País' }],
  },
  categorias: {
    title: 'Categorias', path: 'categorias',
    fields: [
      { n: 'nome', l: 'Nome', req: 1 }, { n: 'descricao', l: 'Descrição' },
      { n: 'valorDiariaBase', l: 'Valor da diária base (R$)', t: 'number', step: '0.01', req: 1 },
    ],
    cols: ['id', 'nome', 'descricao', 'valorDiariaBase'],
    filters: [{ n: 'nome', l: 'Nome' }, { n: 'descricao', l: 'Descrição' }],
  },
  clientes: {
    title: 'Clientes', path: 'clientes',
    fields: [
      { n: 'nome', l: 'Nome', req: 1 },
      { n: 'cpf', l: 'CPF (11 dígitos, só números)', req: 1, max: 11 },
      { n: 'email', l: 'E-mail', t: 'email', req: 1 }, { n: 'telefone', l: 'Telefone' },
    ],
    cols: ['id', 'nome', 'cpf', 'email', 'telefone'],
    filters: [{ n: 'nome', l: 'Nome' }, { n: 'cpf', l: 'CPF' }, { n: 'email', l: 'E-mail' }],
    queries: [
      { label: 'Histórico de aluguéis do cliente',
        params: [{ n: 'id', l: 'Cliente', from: 'clientes', lbl: r => r.nome, req: 1 }],
        url: p => `clientes/${p.id}/historico`, resultado: 'alugueis' },
    ],
  },
  alugueis: {
    title: 'Aluguéis', path: 'alugueis', canEdit: r => !r.dataDevolucao,
    fields: [
      { n: 'clienteId', l: 'Cliente', t: 'select', from: 'clientes', lbl: r => r.nome, req: 1, only: 'new' },
      { n: 'veiculoId', l: 'Veículo (somente disponíveis)', t: 'select', from: 'veiculos/disponiveis', lbl: r => `${r.modelo} (${r.placa})`, req: 1, only: 'new' },
      { n: 'dataRetirada', l: 'Retirada', t: 'datetime-local', req: 1 },
      { n: 'dataPrevistaDevolucao', l: 'Devolução prevista', t: 'datetime-local', req: 1 },
      { n: 'valorDiaria', l: 'Valor da diária (R$)', t: 'number', step: '0.01', req: 1 },
    ],
    cols: ['id', 'clienteNome', 'veiculoModelo', 'veiculoPlaca', 'dataRetirada', 'dataPrevistaDevolucao', 'dataDevolucao', 'valorTotal'],
    filters: [{ n: 'clienteNome', l: 'Cliente' }, { n: 'veiculoPlaca', l: 'Placa' }, { n: 'veiculoModelo', l: 'Veículo' }],
    queries: [
      { label: 'Aluguéis em aberto', params: [], url: () => 'alugueis/em-aberto' },
      { label: 'Aluguéis por período de retirada',
        params: [{ n: 'inicio', l: 'De', t: 'date', req: 1 }, { n: 'fim', l: 'Até', t: 'date', req: 1 }],
        url: p => `alugueis/por-periodo?inicio=${p.inicio}&fim=${p.fim}T23:59:59` },
    ],
    // Editar = alterar datas/diária de aluguel em aberto (PUT /api/alugueis/{id}); Devolução = PUT /api/alugueis/{id}/devolucao
    actions: [{
      l: 'Devolução', show: r => !r.dataDevolucao,
      fields: [
        { n: 'dataDevolucao', l: 'Data da devolução', t: 'datetime-local', req: 1 },
        { n: 'quilometragemFinal', l: 'Quilometragem final', t: 'number', req: 1 },
      ],
      run: (r, b) => ({ path: `alugueis/${r.id}/devolucao`, method: 'PUT', body: b }),
    }],
  },
};