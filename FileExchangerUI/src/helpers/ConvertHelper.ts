export class ConvertHelper{
  static ConvertSize(size: number): string{
    let kb = size / 1024;
    let b = size % 1024;
    let mb = kb / 1024;
    kb = kb % 1024;
    let gb = mb / 1024;
    let res = "";
    gb = Number(gb.toFixed(2));
    kb = Number(kb.toFixed(2));
    mb = Number(mb.toFixed(2));
    if(gb > 1)
      res = `${gb}Gb`;
    else if(mb > 1)
      res = `${mb}Mb`;
    else if(kb > 1)
      res = `${kb}Kb`;
    else
      res = `${b}b`;
    return res;
  }
}
