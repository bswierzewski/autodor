module.exports = {
  autodor: {
    input: {
      target: 'http://localhost:7000/swagger/v1/swagger.json'
    },
    output: {
      mode: 'tags',
      client: 'react-query',
      target: 'src/lib/api/endpoints',
      schemas: 'src/lib/api/models',
      prettier: true,
      override: {
        mutator: {
          path: 'src/lib/api/axios.ts',
          name: 'customInstance'
        }
      }
    }
  }
};
