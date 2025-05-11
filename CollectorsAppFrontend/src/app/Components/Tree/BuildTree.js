export const buildTree = (items, parentId = 0) =>
    items
      .filter((item) => item.parentId === parentId)
      .sort((a, b) => a.name.localeCompare(b.name))
      .map((item) => ({
        ...item,
        children: buildTree(items, item.id),
      }));